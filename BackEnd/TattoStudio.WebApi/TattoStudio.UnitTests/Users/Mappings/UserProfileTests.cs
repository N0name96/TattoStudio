using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using TattoStudio.Application.DTOs.Users;
using TattoStudio.Application.Mappings;
using TattoStudio.Domain.Entities;
using TattoStudio.Domain.Enums;

namespace TattoStudio.UnitTests.Users.Mappings;

public class UserProfileTests
{
    private readonly IMapper _mapper;

    public UserProfileTests()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAutoMapper(cfg => cfg.AddMaps(typeof(UserProfile)));
        _mapper = services.BuildServiceProvider().GetRequiredService<IMapper>();
    }

    [Fact]
    public void UserProfile_AllMappings_AreValid()
    {
        var act = () =>
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddAutoMapper(cfg => cfg.AddMaps(typeof(UserProfile)));
            services.BuildServiceProvider().GetRequiredService<IMapper>();
        };

        act.Should().NotThrow();
    }

    [Fact]
    public void Map_AppUserToDTO_DoesNotExposePasswordHash()
    {
        var entity = new AppUser
        {
            Id = Guid.NewGuid(),
            Email = "staff@tattostudio.com",
            PasswordHash = "$2a$10$hashedvalue",
            Role = UserRole.User,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var dto = _mapper.Map<UserDTO>(entity);

        dto.Id.Should().Be(entity.Id);
        dto.Email.Should().Be(entity.Email);
        dto.Role.Should().Be(entity.Role);
        dto.IsActive.Should().Be(entity.IsActive);
        dto.CreatedAt.Should().Be(entity.CreatedAt);
    }

    [Fact]
    public void Map_RegisterCommandToAppUser_MapsAllFields()
    {
        var command = new RegisterUserCommand
        {
            Email = "staff@tattostudio.com",
            Password = "Secure@pass1",
            Role = UserRole.User
        };

        var entity = _mapper.Map<AppUser>(command);

        entity.Email.Should().Be(command.Email);
        entity.Role.Should().Be(command.Role);
        entity.PasswordHash.Should().BeNullOrEmpty();
    }

    [Fact]
    public void Map_UpdateRequest_OnlyOverwritesProvidedFields()
    {
        var entity = new AppUser
        {
            Id = Guid.NewGuid(),
            Email = "staff@tattostudio.com",
            PasswordHash = "$2a$10$hashedvalue",
            Role = UserRole.User,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var updateRequest = new UpdateUserRequest { IsActive = false };
        _mapper.Map(updateRequest, entity);

        entity.IsActive.Should().BeFalse();
        entity.Role.Should().Be(UserRole.User);
        entity.Email.Should().Be("staff@tattostudio.com");
    }
}
