using FluentAssertions;
using Moq;
using TattoStudio.Application.DTOs.Artists;
using TattoStudio.Application.Interfaces;
using TattoStudio.Application.UsesCases.Commands.Artists;

namespace TattoStudio.UnitTests.Artists.Handlers;

public class CreateArtistHandlerTests
{
    private readonly Mock<IArtistRepository> _repoMock;
    private readonly CreateArtistHandler _handler;

    public CreateArtistHandlerTests()
    {
        _repoMock = new Mock<IArtistRepository>();
        _handler = new CreateArtistHandler(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsRepositoryResult()
    {
        var request = new CreateArtistCommand
        {
            Name = "Laura",
            Surname = "Gómez",
            Mail = "laura@tattostudio.com",
            PhoneNumber = "600111222",
            Comision = 15.50m
        };

        var expected = new ArtistDTO { Id = Guid.NewGuid(), Name = request.Name, Surname = request.Surname, Mail = request.Mail, Comision = request.Comision };

        _repoMock.Setup(r => r.CreateAsync(request, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(expected);

        var result = await _handler.Handle(request, CancellationToken.None);

        result.Should().BeEquivalentTo(expected);
        _repoMock.Verify(r => r.CreateAsync(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_RepositoryThrows_PropagatesException()
    {
        var request = new CreateArtistCommand
        {
            Name = "Test",
            Surname = "Artist",
            Mail = "test@test.com",
            Comision = 10m
        };

        _repoMock.Setup(r => r.CreateAsync(request, It.IsAny<CancellationToken>()))
                 .ThrowsAsync(new Exception("DB error"));

        var act = async () => await _handler.Handle(request, CancellationToken.None);

        await act.Should().ThrowAsync<Exception>().WithMessage("DB error");
    }
}
