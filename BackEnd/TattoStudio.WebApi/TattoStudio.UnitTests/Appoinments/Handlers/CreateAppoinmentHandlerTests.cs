using FluentAssertions;
using Moq;
using TattoStudio.Application.DTOs.Appoinments;
using TattoStudio.Application.Interfaces;
using TattoStudio.Application.UsesCases.Commands.Appoinments;

namespace TattoStudio.UnitTests.Appoinments.Handlers;

public class CreateAppoinmentHandlerTests
{
    private readonly Mock<IAppoinmentRepository> _repoMock;
    private readonly CreateAppoinmentHandler _handler;

    public CreateAppoinmentHandlerTests()
    {
        _repoMock = new Mock<IAppoinmentRepository>();
        _handler = new CreateAppoinmentHandler(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsRepositoryResult()
    {
        var request = new CreateAppoinmentCommand
        {
            ArtistId = Guid.NewGuid(),
            Name = "Juan García",
            MailClient = "juan@test.com",
            TotalPrice = 150m,
            AppoinmentDate = DateTime.UtcNow.AddDays(5)
        };

        var expected = new AppoinmentDTO { Id = Guid.NewGuid(), Name = request.Name };

        _repoMock.Setup(r => r.CreateAsync(request, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(expected);

        var result = await _handler.Handle(request, CancellationToken.None);

        result.Should().BeEquivalentTo(expected);
        _repoMock.Verify(r => r.CreateAsync(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_RepositoryThrows_PropagatesException()
    {
        var request = new CreateAppoinmentCommand
        {
            Name = "Test",
            MailClient = "test@test.com"
        };

        _repoMock.Setup(r => r.CreateAsync(request, It.IsAny<CancellationToken>()))
                 .ThrowsAsync(new Exception("DB error"));

        var act = async () => await _handler.Handle(request, CancellationToken.None);

        await act.Should().ThrowAsync<Exception>().WithMessage("DB error");
    }
}
