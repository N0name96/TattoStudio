using FluentAssertions;
using Moq;
using TattoStudio.Application.DTOs.StudioSettings;
using TattoStudio.Application.Interfaces;
using TattoStudio.Application.UsesCases.Commands.StudioSettings;
using TattoStudio.Domain.Exceptions;

namespace TattoStudio.UnitTests.StudioSettings.Handlers;

public class DeleteStudioSettingsHandlerTests
{
    private readonly Mock<IStudioSettingsRepository> _repoMock;
    private readonly DeleteStudioSettingsHandler _handler;

    public DeleteStudioSettingsHandlerTests()
    {
        _repoMock = new Mock<IStudioSettingsRepository>();
        _handler = new DeleteStudioSettingsHandler(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingId_DeletesAndReturnsUnit()
    {
        var id = Guid.NewGuid();
        _repoMock.Setup(r => r.DeleteAsync(id, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var result = await _handler.Handle(new DeleteStudioSettingsCommand(id), CancellationToken.None);

        result.Should().Be(MediatR.Unit.Value);
        _repoMock.Verify(r => r.DeleteAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NotFound_PropagatesNotFoundException()
    {
        var id = Guid.NewGuid();
        _repoMock.Setup(r => r.DeleteAsync(id, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new StudioSettingsNotFoundException(id));

        var act = async () => await _handler.Handle(new DeleteStudioSettingsCommand(id), CancellationToken.None);

        await act.Should().ThrowAsync<StudioSettingsNotFoundException>();
    }
}
