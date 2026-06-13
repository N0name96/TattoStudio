using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TattoStudio.Application.DTOs.Users;
using TattoStudio.Application.Mappings;
using TattoStudio.Domain.Entities;
using TattoStudio.Domain.Enums;
using TattoStudio.Domain.Exceptions;
using TattoStudio.Infraestructure.Persistence;
using TattoStudio.Infraestructure.Repositories.Users;
using TattoStudio.Infraestructure.Services;

namespace TattoStudio.IntegrationTests.Users;

public class UserRepositoryIntegrationTests
{
    private AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private IMapper CreateMapper()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAutoMapper(cfg => cfg.AddMaps(typeof(UserProfile)));
        return services.BuildServiceProvider().GetRequiredService<IMapper>();
    }

    // Low work factor for test speed
    private readonly BcryptPasswordHasher _hasher = new();

    private Application.Interfaces.IJwtService CreateJwtService()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:SecretKey"] = "test-secret-key-that-is-long-enough-32chars",
                ["Jwt:Issuer"] = "TattoStudio",
                ["Jwt:Audience"] = "TattoStudioApp",
                ["Jwt:ExpirationHours"] = "8"
            })
            .Build();
        return new JwtService(config);
    }

    private AppUser BuildUser(string email, string plainPassword = "Secure@pass1", UserRole role = UserRole.User, bool isActive = true)
    {
        return new AppUser
        {
            Id = Guid.NewGuid(),
            Email = email.ToLowerInvariant(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(plainPassword, 4),
            Role = role,
            IsActive = isActive,
            CreatedAt = DateTime.UtcNow
        };
    }

    // ── Happy paths ────────────────────────────────────────────────────────────

    [Fact]
    public async Task RegisterAsync_ValidRequest_PersistsAndReturnsDTO_WithoutPasswordHash()
    {
        using var context = CreateContext();
        var repo = new UserRepository(context, CreateMapper(), _hasher, CreateJwtService());

        var request = new RegisterUserCommand
        {
            Email = "staff@tattostudio.com",
            Password = "Secure@pass1",
            Role = UserRole.User
        };

        var result = await repo.RegisterAsync(request, CancellationToken.None);

        result.Should().NotBeNull();
        result.Email.Should().Be("staff@tattostudio.com");
        result.Role.Should().Be(UserRole.User);
        result.IsActive.Should().BeTrue();
        context.Users.Count().Should().Be(1);
    }

    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsLoginResponse()
    {
        using var context = CreateContext();
        context.Users.Add(BuildUser("staff@tattostudio.com", "Secure@pass1"));
        await context.SaveChangesAsync();

        var repo = new UserRepository(context, CreateMapper(), _hasher, CreateJwtService());

        var result = await repo.LoginAsync(new LoginUserCommand
        {
            Email = "staff@tattostudio.com",
            Password = "Secure@pass1"
        }, CancellationToken.None);

        result.Should().NotBeNull();
        result.Token.Should().NotBeNullOrEmpty();
        result.ExpiresAt.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsDTO_WithoutPasswordHash()
    {
        using var context = CreateContext();
        var user = BuildUser("staff@tattostudio.com");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var repo = new UserRepository(context, CreateMapper(), _hasher, CreateJwtService());

        var result = await repo.GetByIdAsync(user.Id, CancellationToken.None);

        result.Id.Should().Be(user.Id);
        result.Email.Should().Be(user.Email);
    }

    [Fact]
    public async Task GetAllAsync_WithMultipleUsers_ReturnsAll()
    {
        using var context = CreateContext();
        context.Users.AddRange(
            BuildUser("a@test.com"),
            BuildUser("b@test.com"),
            BuildUser("c@test.com"));
        await context.SaveChangesAsync();

        var repo = new UserRepository(context, CreateMapper(), _hasher, CreateJwtService());

        var result = await repo.GetAllAsync(CancellationToken.None);

        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetAllAsync_EmptyDatabase_ReturnsEmptyList()
    {
        using var context = CreateContext();
        var repo = new UserRepository(context, CreateMapper(), _hasher, CreateJwtService());

        var result = await repo.GetAllAsync(CancellationToken.None);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task UpdateAsync_ExistingId_UpdatesOnlyProvidedFields()
    {
        using var context = CreateContext();
        var user = BuildUser("staff@tattostudio.com");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var repo = new UserRepository(context, CreateMapper(), _hasher, CreateJwtService());

        var result = await repo.UpdateAsync(user.Id, new UpdateUserRequest { IsActive = false }, CancellationToken.None);

        result.IsActive.Should().BeFalse();
        result.Email.Should().Be(user.Email);
        result.Role.Should().Be(user.Role);
    }

    [Fact]
    public async Task DeleteAsync_ExistingId_RemovesFromDatabase()
    {
        using var context = CreateContext();
        var user = BuildUser("staff@tattostudio.com");
        var admin = BuildUser("admin@tattostudio.com", role: UserRole.Admin);
        context.Users.AddRange(user, admin);
        await context.SaveChangesAsync();

        var repo = new UserRepository(context, CreateMapper(), _hasher, CreateJwtService());

        await repo.DeleteAsync(user.Id, admin.Id, CancellationToken.None);

        context.Users.Count().Should().Be(1);
    }

    // ── Bad paths ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task RegisterAsync_DuplicateEmail_ThrowsEmailConflictException()
    {
        using var context = CreateContext();
        context.Users.Add(BuildUser("dup@test.com"));
        await context.SaveChangesAsync();

        var repo = new UserRepository(context, CreateMapper(), _hasher, CreateJwtService());

        var act = async () => await repo.RegisterAsync(new RegisterUserCommand
        {
            Email = "dup@test.com",
            Password = "Secure@pass1"
        }, CancellationToken.None);

        await act.Should().ThrowAsync<UserEmailConflictException>();
    }

    [Fact]
    public async Task LoginAsync_WrongPassword_ThrowsInvalidCredentialsException()
    {
        using var context = CreateContext();
        context.Users.Add(BuildUser("staff@tattostudio.com", "Secure@pass1"));
        await context.SaveChangesAsync();

        var repo = new UserRepository(context, CreateMapper(), _hasher, CreateJwtService());

        var act = async () => await repo.LoginAsync(new LoginUserCommand
        {
            Email = "staff@tattostudio.com",
            Password = "Wrong@pass99"
        }, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidCredentialsException>();
    }

    [Fact]
    public async Task LoginAsync_EmailNotFound_ThrowsInvalidCredentialsException()
    {
        using var context = CreateContext();
        var repo = new UserRepository(context, CreateMapper(), _hasher, CreateJwtService());

        var act = async () => await repo.LoginAsync(new LoginUserCommand
        {
            Email = "noexiste@test.com",
            Password = "Secure@pass1"
        }, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidCredentialsException>();
    }

    [Fact]
    public async Task LoginAsync_InactiveUser_ThrowsUserInactiveException()
    {
        using var context = CreateContext();
        context.Users.Add(BuildUser("inactive@test.com", isActive: false));
        await context.SaveChangesAsync();

        var repo = new UserRepository(context, CreateMapper(), _hasher, CreateJwtService());

        var act = async () => await repo.LoginAsync(new LoginUserCommand
        {
            Email = "inactive@test.com",
            Password = "Secure@pass1"
        }, CancellationToken.None);

        await act.Should().ThrowAsync<UserInactiveException>();
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingId_ThrowsNotFoundException()
    {
        using var context = CreateContext();
        var repo = new UserRepository(context, CreateMapper(), _hasher, CreateJwtService());

        var act = async () => await repo.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        await act.Should().ThrowAsync<UserNotFoundException>();
    }

    [Fact]
    public async Task UpdateAsync_NonExistingId_ThrowsNotFoundException()
    {
        using var context = CreateContext();
        var repo = new UserRepository(context, CreateMapper(), _hasher, CreateJwtService());

        var act = async () => await repo.UpdateAsync(Guid.NewGuid(), new UpdateUserRequest { IsActive = false }, CancellationToken.None);

        await act.Should().ThrowAsync<UserNotFoundException>();
    }

    [Fact]
    public async Task DeleteAsync_NonExistingId_ThrowsNotFoundException()
    {
        using var context = CreateContext();
        var repo = new UserRepository(context, CreateMapper(), _hasher, CreateJwtService());
        var adminId = Guid.NewGuid();

        var act = async () => await repo.DeleteAsync(Guid.NewGuid(), adminId, CancellationToken.None);

        await act.Should().ThrowAsync<UserNotFoundException>();
    }

    [Fact]
    public async Task DeleteAsync_SelfDelete_ThrowsSelfDeleteException()
    {
        using var context = CreateContext();
        var admin = BuildUser("admin@test.com", role: UserRole.Admin);
        context.Users.Add(admin);
        await context.SaveChangesAsync();

        var repo = new UserRepository(context, CreateMapper(), _hasher, CreateJwtService());

        var act = async () => await repo.DeleteAsync(admin.Id, admin.Id, CancellationToken.None);

        await act.Should().ThrowAsync<UserSelfDeleteException>();
    }
}
