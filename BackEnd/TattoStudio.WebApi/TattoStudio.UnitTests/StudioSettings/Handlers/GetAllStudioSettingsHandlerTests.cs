using FluentAssertions;
using Moq;
using TattoStudio.Application.DTOs.StudioSettings;
using TattoStudio.Application.Interfaces;
using TattoStudio.Application.UsesCases.Queries.StudioSettings;

namespace TattoStudio.UnitTests.StudioSettings.Handlers;

public class GetAllStudioSettingsHandlerTests
{
    private readonly Mock<IStudioSettingsRepository> _repoMock;
    private readonly GetAllStudioSettingsHandler _handler;

    public GetAllStudioSettingsHandlerTests()
    {
        _repoMock = new Mock<IStudioSettingsRepository>();
        _handler = new GetAllStudioSettingsHandler(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_WithSettings_ReturnsList()
    {
        var settings = new List<StudioSettingsDTO>
        {
            new() { Id = Guid.NewGuid(), WorkdayStart = new TimeOnly(9, 0), WorkdayEnd = new TimeOnly(18, 0) }
        };
        _repoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(settings);

        var result = await _handler.Handle(new GetAllStudioSettingsQuery(), CancellationToken.None);

        result.Should().HaveCount(1);
        result.First().WorkdayStart.Should().Be(new TimeOnly(9, 0));
    }

    [Fact]
    public async Task Handle_NoSettings_ReturnsEmptyList()
    {
        _repoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync([]);

        var result = await _handler.Handle(new GetAllStudioSettingsQuery(), CancellationToken.None);

        result.Should().BeEmpty();
    }
}
