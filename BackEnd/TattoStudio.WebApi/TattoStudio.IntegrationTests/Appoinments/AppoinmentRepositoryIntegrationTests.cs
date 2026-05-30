using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TattoStudio.Application.DTOs.Appoinments;
using TattoStudio.Application.Mappings;
using TattoStudio.Domain.Entities;
using TattoStudio.Domain.Exceptions;
using TattoStudio.Infraestructure.Persistence;
using TattoStudio.Infraestructure.Repositories.Appoinments;

namespace TattoStudio.IntegrationTests.Appoinments;

public class AppoinmentRepositoryIntegrationTests
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
        services.AddAutoMapper(cfg => cfg.AddMaps(typeof(AppoinmentProfile)));
        return services.BuildServiceProvider().GetRequiredService<IMapper>();
    }

    private static Appoinment BuildEntity(string name = "Test Client") => new()
    {
        Id = Guid.NewGuid(),
        ArtistId = Guid.NewGuid(),
        Name = name,
        MailClient = "test@test.com",
        TotalPrice = 150m,
        AppoinmentDate = DateTime.UtcNow.AddDays(7),
        CreatedAt = DateTime.UtcNow
    };

    // ── Happy paths ────────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_ValidRequest_PersistsAndReturnsDTO()
    {
        using var context = CreateContext();
        var repo = new AppoinmentRepository(context, CreateMapper());

        var request = new CreateAppoinmentCommand
        {
            ArtistId = Guid.NewGuid(),
            Name = "María Fernández",
            MailClient = "maria@test.com",
            TotalPrice = 200m,
            AppoinmentDate = DateTime.UtcNow.AddDays(10)
        };

        var result = await repo.CreateAsync(request, CancellationToken.None);

        result.Should().NotBeNull();
        result.Name.Should().Be(request.Name);
        result.MailClient.Should().Be(request.MailClient);
        result.TotalPrice.Should().Be(request.TotalPrice);
        context.Appoinments.Count().Should().Be(1);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsCorrectDTO()
    {
        using var context = CreateContext();
        var entity = BuildEntity("Pedro Sanz");
        context.Appoinments.Add(entity);
        await context.SaveChangesAsync();

        var repo = new AppoinmentRepository(context, CreateMapper());

        var result = await repo.GetByIdAsync(entity.Id, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(entity.Id);
        result.Name.Should().Be("Pedro Sanz");
    }

    [Fact]
    public async Task GetAllAsync_WithMultipleEntities_ReturnsAll()
    {
        using var context = CreateContext();
        context.Appoinments.AddRange(BuildEntity("A"), BuildEntity("B"), BuildEntity("C"));
        await context.SaveChangesAsync();

        var repo = new AppoinmentRepository(context, CreateMapper());

        var result = await repo.GetAllAsync(CancellationToken.None);

        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task DeleteAsync_ExistingId_RemovesFromDatabase()
    {
        using var context = CreateContext();
        var entity = BuildEntity();
        context.Appoinments.Add(entity);
        await context.SaveChangesAsync();

        var repo = new AppoinmentRepository(context, CreateMapper());

        await repo.DeleteAsync(entity.Id, CancellationToken.None);

        context.Appoinments.Count().Should().Be(0);
    }

    [Fact]
    public async Task UpdateAsync_ExistingId_UpdatesOnlyProvidedFields()
    {
        using var context = CreateContext();
        var entity = BuildEntity("Original Name");
        entity.MailClient = "original@test.com";
        entity.TotalPrice = 100m;
        context.Appoinments.Add(entity);
        await context.SaveChangesAsync();

        var repo = new AppoinmentRepository(context, CreateMapper());

        var updateRequest = new UpdateAppoinmentRequest { Name = "Updated Name" };
        var result = await repo.UpdateAsync(entity.Id, updateRequest, CancellationToken.None);

        result.Should().NotBeNull();
        result!.Name.Should().Be("Updated Name");
        result.MailClient.Should().Be("original@test.com");
        result.TotalPrice.Should().Be(100m);
    }

    [Fact]
    public async Task CreateAsync_SetsCreatedAtAutomatically()
    {
        using var context = CreateContext();
        var repo = new AppoinmentRepository(context, CreateMapper());
        var before = DateTime.UtcNow;

        var request = new CreateAppoinmentCommand
        {
            ArtistId = Guid.NewGuid(),
            Name = "Luis Mora",
            MailClient = "luis@test.com",
            AppoinmentDate = DateTime.UtcNow.AddDays(3)
        };

        var result = await repo.CreateAsync(request, CancellationToken.None);

        result.CreatedAt.Should().BeOnOrAfter(before);
    }

    [Fact]
    public async Task CreateAsync_DuplicateArtistAndDate_ThrowsConflictException()
    {
        using var context = CreateContext();
        var repo = new AppoinmentRepository(context, CreateMapper());
        var artistId = Guid.NewGuid();
        var date = DateTime.UtcNow.AddDays(5);

        await repo.CreateAsync(new CreateAppoinmentCommand
        {
            ArtistId = artistId,
            Name = "First Client",
            MailClient = "first@test.com",
            AppoinmentDate = date
        }, CancellationToken.None);

        var act = async () => await repo.CreateAsync(new CreateAppoinmentCommand
        {
            ArtistId = artistId,
            Name = "Second Client",
            MailClient = "second@test.com",
            AppoinmentDate = date
        }, CancellationToken.None);

        await act.Should().ThrowAsync<AppoinmentConflictException>();
    }

    [Fact]
    public async Task UpdateAsync_MovingToConflictingSlot_ThrowsConflictException()
    {
        using var context = CreateContext();
        var repo = new AppoinmentRepository(context, CreateMapper());
        var artistId = Guid.NewGuid();
        var takenDate = DateTime.UtcNow.AddDays(5);

        var taken = BuildEntity("Taken");
        taken.ArtistId = artistId;
        taken.AppoinmentDate = takenDate;
        context.Appoinments.Add(taken);

        var toMove = BuildEntity("To Move");
        toMove.ArtistId = artistId;
        toMove.AppoinmentDate = DateTime.UtcNow.AddDays(10);
        context.Appoinments.Add(toMove);

        await context.SaveChangesAsync();

        var act = async () => await repo.UpdateAsync(
            toMove.Id,
            new UpdateAppoinmentRequest { AppoinmentDate = takenDate },
            CancellationToken.None);

        await act.Should().ThrowAsync<AppoinmentConflictException>();
    }

    // ── Bad paths ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetByIdAsync_NonExistingId_ThrowsNotFoundException()
    {
        using var context = CreateContext();
        var repo = new AppoinmentRepository(context, CreateMapper());

        var act = async () => await repo.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        await act.Should().ThrowAsync<AppoinmentNotFoundException>();
    }

    [Fact]
    public async Task DeleteAsync_NonExistingId_DoesNotThrow()
    {
        using var context = CreateContext();
        var repo = new AppoinmentRepository(context, CreateMapper());

        var act = async () => await repo.DeleteAsync(Guid.NewGuid(), CancellationToken.None);

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task UpdateAsync_NonExistingId_ThrowsNotFoundException()
    {
        using var context = CreateContext();
        var repo = new AppoinmentRepository(context, CreateMapper());

        var updateRequest = new UpdateAppoinmentRequest { Name = "Ghost" };
        var act = async () => await repo.UpdateAsync(Guid.NewGuid(), updateRequest, CancellationToken.None);

        await act.Should().ThrowAsync<AppoinmentNotFoundException>();
    }

    [Fact]
    public async Task GetAllAsync_EmptyDatabase_ReturnsEmptyList()
    {
        using var context = CreateContext();
        var repo = new AppoinmentRepository(context, CreateMapper());

        var result = await repo.GetAllAsync(CancellationToken.None);

        result.Should().BeEmpty();
    }
}
