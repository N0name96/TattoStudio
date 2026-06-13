using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TattoStudio.Application.DTOs.Artists;
using TattoStudio.Application.Mappings;
using TattoStudio.Domain.Entities;
using TattoStudio.Domain.Exceptions;
using TattoStudio.Infraestructure.Persistence;
using TattoStudio.Infraestructure.Repositories.Artists;

namespace TattoStudio.IntegrationTests.Artists;

public class ArtistRepositoryIntegrationTests
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
        services.AddAutoMapper(cfg => cfg.AddMaps(typeof(ArtistProfile)));
        return services.BuildServiceProvider().GetRequiredService<IMapper>();
    }

    private static Artist BuildEntity(string name = "Test Artist", string mail = "") => new()
    {
        Id = Guid.NewGuid(),
        Name = name,
        Surname = "Surname",
        Mail = string.IsNullOrEmpty(mail) ? $"{Guid.NewGuid()}@test.com" : mail,
        Comision = 15m
    };

    // ── Happy paths ────────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_ValidRequest_PersistsAndReturnsDTO()
    {
        using var context = CreateContext();
        var repo = new ArtistRepository(context, CreateMapper());

        var request = new CreateArtistCommand
        {
            Name = "Laura",
            Surname = "Gómez",
            Mail = "laura@tattostudio.com",
            PhoneNumber = "600111222",
            Comision = 15.50m
        };

        var result = await repo.CreateAsync(request, CancellationToken.None);

        result.Should().NotBeNull();
        result.Name.Should().Be(request.Name);
        result.Surname.Should().Be(request.Surname);
        result.Mail.Should().Be(request.Mail);
        result.Comision.Should().Be(request.Comision);
        context.Artists.Count().Should().Be(1);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsCorrectDTO()
    {
        using var context = CreateContext();
        var entity = BuildEntity("Pedro", "pedro@test.com");
        context.Artists.Add(entity);
        await context.SaveChangesAsync();

        var repo = new ArtistRepository(context, CreateMapper());

        var result = await repo.GetByIdAsync(entity.Id, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(entity.Id);
        result.Name.Should().Be("Pedro");
    }

    [Fact]
    public async Task GetAllAsync_WithMultipleEntities_ReturnsAll()
    {
        using var context = CreateContext();
        context.Artists.AddRange(
            BuildEntity("A", "a@test.com"),
            BuildEntity("B", "b@test.com"),
            BuildEntity("C", "c@test.com"));
        await context.SaveChangesAsync();

        var repo = new ArtistRepository(context, CreateMapper());

        var result = await repo.GetAllAsync(CancellationToken.None);

        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetAllAsync_EmptyDatabase_ReturnsEmptyList()
    {
        using var context = CreateContext();
        var repo = new ArtistRepository(context, CreateMapper());

        var result = await repo.GetAllAsync(CancellationToken.None);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task UpdateAsync_ExistingId_UpdatesOnlyProvidedFields()
    {
        using var context = CreateContext();
        var entity = BuildEntity("Original", "original@test.com");
        entity.Comision = 10m;
        context.Artists.Add(entity);
        await context.SaveChangesAsync();

        var repo = new ArtistRepository(context, CreateMapper());

        var updateRequest = new UpdateArtistRequest { Name = "Updated" };
        var result = await repo.UpdateAsync(entity.Id, updateRequest, CancellationToken.None);

        result.Should().NotBeNull();
        result.Name.Should().Be("Updated");
        result.Mail.Should().Be("original@test.com");
        result.Comision.Should().Be(10m);
    }

    [Fact]
    public async Task UpdateAsync_SameMailSameArtist_DoesNotThrowConflict()
    {
        using var context = CreateContext();
        var entity = BuildEntity("Laura", "laura@test.com");
        context.Artists.Add(entity);
        await context.SaveChangesAsync();

        var repo = new ArtistRepository(context, CreateMapper());

        var act = async () => await repo.UpdateAsync(
            entity.Id,
            new UpdateArtistRequest { Mail = "laura@test.com", Comision = 20m },
            CancellationToken.None);

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task DeleteAsync_ExistingId_RemovesFromDatabase()
    {
        using var context = CreateContext();
        var entity = BuildEntity();
        context.Artists.Add(entity);
        await context.SaveChangesAsync();

        var repo = new ArtistRepository(context, CreateMapper());

        await repo.DeleteAsync(entity.Id, CancellationToken.None);

        context.Artists.Count().Should().Be(0);
    }

    // ── Bad paths ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_DuplicateMail_ThrowsMailConflictException()
    {
        using var context = CreateContext();
        var repo = new ArtistRepository(context, CreateMapper());

        await repo.CreateAsync(new CreateArtistCommand
        {
            Name = "First",
            Surname = "Artist",
            Mail = "dup@test.com",
            Comision = 10m
        }, CancellationToken.None);

        var act = async () => await repo.CreateAsync(new CreateArtistCommand
        {
            Name = "Second",
            Surname = "Artist",
            Mail = "dup@test.com",
            Comision = 10m
        }, CancellationToken.None);

        await act.Should().ThrowAsync<ArtistMailConflictException>();
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingId_ThrowsNotFoundException()
    {
        using var context = CreateContext();
        var repo = new ArtistRepository(context, CreateMapper());

        var act = async () => await repo.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        await act.Should().ThrowAsync<ArtistNotFoundException>();
    }

    [Fact]
    public async Task UpdateAsync_NonExistingId_ThrowsNotFoundException()
    {
        using var context = CreateContext();
        var repo = new ArtistRepository(context, CreateMapper());

        var act = async () => await repo.UpdateAsync(Guid.NewGuid(), new UpdateArtistRequest { Name = "Ghost" }, CancellationToken.None);

        await act.Should().ThrowAsync<ArtistNotFoundException>();
    }

    [Fact]
    public async Task UpdateAsync_MailUsedByAnotherArtist_ThrowsMailConflictException()
    {
        using var context = CreateContext();
        context.Artists.AddRange(
            BuildEntity("A", "a@test.com"),
            BuildEntity("B", "b@test.com"));
        await context.SaveChangesAsync();

        var repo = new ArtistRepository(context, CreateMapper());
        var artistA = context.Artists.First(a => a.Mail == "a@test.com");

        var act = async () => await repo.UpdateAsync(
            artistA.Id,
            new UpdateArtistRequest { Mail = "b@test.com" },
            CancellationToken.None);

        await act.Should().ThrowAsync<ArtistMailConflictException>();
    }

    [Fact]
    public async Task DeleteAsync_NonExistingId_ThrowsNotFoundException()
    {
        using var context = CreateContext();
        var repo = new ArtistRepository(context, CreateMapper());

        var act = async () => await repo.DeleteAsync(Guid.NewGuid(), CancellationToken.None);

        await act.Should().ThrowAsync<ArtistNotFoundException>();
    }
}
