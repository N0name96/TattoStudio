using FluentAssertions;
using Moq;
using TattoStudio.Application.DTOs.Users;
using TattoStudio.Application.Interfaces;
using TattoStudio.Application.UsesCases.Commands.Users;

namespace TattoStudio.UnitTests.Users.Handlers;

public class RegisterUserHandlerTests
{
    private readonly Mock<IUserRepository> _repoMock;
    private readonly RegisterUserHandler _handler;

    public RegisterUserHandlerTests()
    {
        _repoMock = new Mock<IUserRepository>();
        _handler = new RegisterUserHandler(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsUserDTO()
    {
        var request = new RegisterUserCommand
        {
            Email = "staff@tattostudio.com",
            Password = "Secure@pass1"
        };

        var expected = new UserDTO
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            Role = Domain.Enums.UserRole.User,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _repoMock.Setup(r => r.RegisterAsync(request, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(expected);

        var result = await _handler.Handle(request, CancellationToken.None);

        result.Should().BeEquivalentTo(expected);
        _repoMock.Verify(r => r.RegisterAsync(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_RepositoryThrows_PropagatesException()
    {
        var request = new RegisterUserCommand { Email = "test@test.com", Password = "Test@1234" };

        _repoMock.Setup(r => r.RegisterAsync(request, It.IsAny<CancellationToken>()))
                 .ThrowsAsync(new Exception("DB error"));

        var act = async () => await _handler.Handle(request, CancellationToken.None);

        await act.Should().ThrowAsync<Exception>().WithMessage("DB error");
    }
}
