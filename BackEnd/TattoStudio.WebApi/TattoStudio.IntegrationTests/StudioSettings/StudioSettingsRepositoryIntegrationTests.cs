using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TattoStudio.Application.DTOs.StudioSettings;
using TattoStudio.Application.Mappings;
using TattoStudio.Domain.Exceptions;
using TattoStudio.Infraestructure.Persistence;
using TattoStudio.Infraestructure.Repositories.StudioSettings;

namespace TattoStudio.IntegrationTests.StudioSettings;

public class StudioSettingsRepositoryIntegrationTests
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
        services.AddAutoMapper(cfg => cfg.AddMaps(typeof(StudioSettingsProfile)));
        return services.BuildServiceProvider().GetRequiredService<IMapper>();
    }

    [Fact]
    public async Task Create_ValidData_PersistsAndReturnsDTO()
    {
        using var context = CreateContext();
        var repo = new StudioSettingsRepository(context, CreateMapper());

        var command = new CreateStudioSettingsCommand
        {
            WorkdayStart = new TimeOnly(9, 0),
            WorkdayEnd = new TimeOnly(18, 0)
        };

        var result = await repo.CreateAsync(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().NotBe(Guid.Empty);
        result.WorkdayStart.Should().Be(new TimeOnly(9, 0));
        result.WorkdayEnd.Should().Be(new TimeOnly(18, 0));
        context.StudioSettings.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetAll_Empty_ReturnsEmptyList()
    {
        using var context = CreateContext();
        var repo = new StudioSettingsRepository(context, CreateMapper());

        var result = await repo.GetAllAsync(CancellationToken.None);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAll_WithEntry_ReturnsList()
    {
        using var context = CreateContext();
        var repo = new StudioSettingsRepository(context, CreateMapper());

        await repo.CreateAsync(new CreateStudioSettingsCommand { WorkdayStart = new TimeOnly(8, 0), WorkdayEnd = new TimeOnly(20, 0) }, CancellationToken.None);

        var result = await repo.GetAllAsync(CancellationToken.None);

        result.Should().HaveCount(1);
        result.First().WorkdayStart.Should().Be(new TimeOnly(8, 0));
    }

    [Fact]
    public async Task Update_ValidData_ReturnsUpdatedDTO()
    {
        using var context = CreateContext();
        var repo = new StudioSettingsRepository(context, CreateMapper());

        var created = await repo.CreateAsync(
            new CreateStudioSettingsCommand { WorkdayStart = new TimeOnly(9, 0), WorkdayEnd = new TimeOnly(18, 0) },
            CancellationToken.None);

        var updateRequest = new UpdateStudioSettingsRequest { WorkdayStart = new TimeOnly(10, 0), WorkdayEnd = new TimeOnly(21, 0) };
        var result = await repo.UpdateAsync(created.Id, updateRequest, CancellationToken.None);

        result.WorkdayStart.Should().Be(new TimeOnly(10, 0));
        result.WorkdayEnd.Should().Be(new TimeOnly(21, 0));
    }

    [Fact]
    public async Task Update_NotFound_ThrowsNotFoundException()
    {
        using var context = CreateContext();
        var repo = new StudioSettingsRepository(context, CreateMapper());

        var act = async () => await repo.UpdateAsync(
            Guid.NewGuid(),
            new UpdateStudioSettingsRequest { WorkdayStart = new TimeOnly(9, 0), WorkdayEnd = new TimeOnly(18, 0) },
            CancellationToken.None);

        await act.Should().ThrowAsync<StudioSettingsNotFoundException>();
    }

    [Fact]
    public async Task Delete_Exists_RemovesEntry()
    {
        using var context = CreateContext();
        var repo = new StudioSettingsRepository(context, CreateMapper());

        var created = await repo.CreateAsync(
            new CreateStudioSettingsCommand { WorkdayStart = new TimeOnly(9, 0), WorkdayEnd = new TimeOnly(18, 0) },
            CancellationToken.None);

        await repo.DeleteAsync(created.Id, CancellationToken.None);

        context.StudioSettings.Should().BeEmpty();
    }

    [Fact]
    public async Task Delete_NotFound_ThrowsNotFoundException()
    {
        using var context = CreateContext();
        var repo = new StudioSettingsRepository(context, CreateMapper());

        var act = async () => await repo.DeleteAsync(Guid.NewGuid(), CancellationToken.None);

        await act.Should().ThrowAsync<StudioSettingsNotFoundException>();
    }
}
