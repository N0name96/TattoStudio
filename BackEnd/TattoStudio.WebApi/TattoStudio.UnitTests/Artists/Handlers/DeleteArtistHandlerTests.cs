using FluentAssertions;
using MediatR;
using Moq;
using TattoStudio.Application.Interfaces;
using TattoStudio.Application.UsesCases.Commands.Artists;
using TattoStudio.Domain.Exceptions;

namespace TattoStudio.UnitTests.Artists.Handlers;

public class DeleteArtistHandlerTests
{
    private readonly Mock<IArtistRepository> _repoMock;
    private readonly DeleteArtistHandler _handler;

    public DeleteArtistHandlerTests()
    {
        _repoMock = new Mock<IArtistRepository>();
        _handler = new DeleteArtistHandler(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_ActiveArtist_CallsSoftDeleteAndReturnsUnit()
    {
        var artistId = Guid.NewGuid();
        _repoMock.Setup(r => r.DeleteAsync(artistId, It.IsAny<CancellationToken>()))
                 .Returns(Task.CompletedTask);

        var result = await _handler.Handle(new DeleteArtistCommand(artistId), CancellationToken.None);

        result.Should().Be(Unit.Value);
        _repoMock.Verify(r => r.DeleteAsync(artistId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ArtistNotFound_ThrowsArtistNotFoundException()
    {
        var artistId = Guid.NewGuid();
        _repoMock.Setup(r => r.DeleteAsync(artistId, It.IsAny<CancellationToken>()))
                 .ThrowsAsync(new ArtistNotFoundException(artistId));

        var act = async () => await _handler.Handle(new DeleteArtistCommand(artistId), CancellationToken.None);

        await act.Should().ThrowAsync<ArtistNotFoundException>();
    }

    [Fact]
    public async Task Handle_ArtistAlreadyInactive_ThrowsArtistAlreadyInactiveException()
    {
        var artistId = Guid.NewGuid();
        _repoMock.Setup(r => r.DeleteAsync(artistId, It.IsAny<CancellationToken>()))
                 .ThrowsAsync(new ArtistAlreadyInactiveException(artistId));

        var act = async () => await _handler.Handle(new DeleteArtistCommand(artistId), CancellationToken.None);

        await act.Should().ThrowAsync<ArtistAlreadyInactiveException>();
    }
}
