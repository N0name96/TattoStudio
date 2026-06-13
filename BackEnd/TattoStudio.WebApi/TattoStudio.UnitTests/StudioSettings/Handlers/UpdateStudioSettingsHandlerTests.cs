using FluentAssertions;
using Moq;
using TattoStudio.Application.DTOs.StudioSettings;
using TattoStudio.Application.Interfaces;
using TattoStudio.Application.UsesCases.Commands.StudioSettings;
using TattoStudio.Domain.Exceptions;

namespace TattoStudio.UnitTests.StudioSettings.Handlers;

public class UpdateStudioSettingsHandlerTests
{
    private readonly Mock<IStudioSettingsRepository> _repoMock;
    private readonly UpdateStudioSettingsHandler _handler;

    public UpdateStudioSettingsHandlerTests()
    {
        _repoMock = new Mock<IStudioSettingsRepository>();
        _handler = new UpdateStudioSettingsHandler(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_ValidData_ReturnsUpdatedDTO()
    {
        var id = Guid.NewGuid();
        var request = new UpdateStudioSettingsRequest { WorkdayStart = new TimeOnly(10, 0), WorkdayEnd = new TimeOnly(21, 0) };
        var command = new UpdateStudioSettingsCommand(id, request);
        var expected = new StudioSettingsDTO { Id = id, WorkdayStart = request.WorkdayStart, WorkdayEnd = request.WorkdayEnd };

        _repoMock.Setup(r => r.UpdateAsync(id, request, It.IsAny<CancellationToken>())).ReturnsAsync(expected);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task Handle_NotFound_PropagatesNotFoundException()
    {
        var id = Guid.NewGuid();
        var request = new UpdateStudioSettingsRequest { WorkdayStart = new TimeOnly(9, 0), WorkdayEnd = new TimeOnly(18, 0) };

        _repoMock.Setup(r => r.UpdateAsync(id, request, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new StudioSettingsNotFoundException(id));

        var act = async () => await _handler.Handle(new UpdateStudioSettingsCommand(id, request), CancellationToken.None);

        await act.Should().ThrowAsync<StudioSettingsNotFoundException>();
    }

    [Fact]
    public async Task Handle_WorkdayEndNotAfterStart_ThrowsInvalidScheduleException()
    {
        var request = new UpdateStudioSettingsRequest { WorkdayStart = new TimeOnly(18, 0), WorkdayEnd = new TimeOnly(8, 0) };

        var act = async () => await _handler.Handle(new UpdateStudioSettingsCommand(Guid.NewGuid(), request), CancellationToken.None);

        await act.Should().ThrowAsync<StudioSettingsInvalidScheduleException>();
        _repoMock.Verify(r => r.UpdateAsync(It.IsAny<Guid>(), It.IsAny<UpdateStudioSettingsRequest>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
