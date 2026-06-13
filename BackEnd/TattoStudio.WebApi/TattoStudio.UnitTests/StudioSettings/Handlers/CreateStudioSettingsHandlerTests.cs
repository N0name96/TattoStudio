using FluentAssertions;
using Moq;
using TattoStudio.Application.DTOs.StudioSettings;
using TattoStudio.Application.Interfaces;
using TattoStudio.Application.UsesCases.Commands.StudioSettings;
using TattoStudio.Domain.Exceptions;

namespace TattoStudio.UnitTests.StudioSettings.Handlers;

public class CreateStudioSettingsHandlerTests
{
    private readonly Mock<IStudioSettingsRepository> _repoMock;
    private readonly CreateStudioSettingsHandler _handler;

    public CreateStudioSettingsHandlerTests()
    {
        _repoMock = new Mock<IStudioSettingsRepository>();
        _handler = new CreateStudioSettingsHandler(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_ValidData_ReturnsCreatedDTO()
    {
        var command = new CreateStudioSettingsCommand
        {
            WorkdayStart = new TimeOnly(9, 0),
            WorkdayEnd = new TimeOnly(18, 0)
        };
        var expected = new StudioSettingsDTO { Id = Guid.NewGuid(), WorkdayStart = command.WorkdayStart, WorkdayEnd = command.WorkdayEnd };

        _repoMock.Setup(r => r.CreateAsync(command, It.IsAny<CancellationToken>())).ReturnsAsync(expected);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeEquivalentTo(expected);
        _repoMock.Verify(r => r.CreateAsync(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WorkdayEndNotAfterStart_ThrowsInvalidScheduleException()
    {
        var command = new CreateStudioSettingsCommand
        {
            WorkdayStart = new TimeOnly(18, 0),
            WorkdayEnd = new TimeOnly(9, 0)
        };

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<StudioSettingsInvalidScheduleException>();
        _repoMock.Verify(r => r.CreateAsync(It.IsAny<CreateStudioSettingsCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WorkdayEndEqualToStart_ThrowsInvalidScheduleException()
    {
        var command = new CreateStudioSettingsCommand
        {
            WorkdayStart = new TimeOnly(9, 0),
            WorkdayEnd = new TimeOnly(9, 0)
        };

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<StudioSettingsInvalidScheduleException>();
    }
}
