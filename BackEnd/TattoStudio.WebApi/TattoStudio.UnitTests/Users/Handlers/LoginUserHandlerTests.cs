using FluentAssertions;
using Moq;
using TattoStudio.Application.DTOs.Users;
using TattoStudio.Application.Interfaces;
using TattoStudio.Application.UsesCases.Commands.Users;

namespace TattoStudio.UnitTests.Users.Handlers;

public class LoginUserHandlerTests
{
    private readonly Mock<IUserRepository> _repoMock;
    private readonly LoginUserHandler _handler;

    public LoginUserHandlerTests()
    {
        _repoMock = new Mock<IUserRepository>();
        _handler = new LoginUserHandler(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsLoginResponse()
    {
        var request = new LoginUserCommand
        {
            Email = "staff@tattostudio.com",
            Password = "Secure@pass1"
        };

        var expected = new LoginResponseDTO
        {
            Token = "eyJhbGciOiJIUzI1NiJ9.test.sig",
            ExpiresAt = DateTime.UtcNow.AddHours(8)
        };

        _repoMock.Setup(r => r.LoginAsync(request, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(expected);

        var result = await _handler.Handle(request, CancellationToken.None);

        result.Should().BeEquivalentTo(expected);
        _repoMock.Verify(r => r.LoginAsync(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_RepositoryThrows_PropagatesException()
    {
        var request = new LoginUserCommand { Email = "bad@test.com", Password = "Wrong@pass1" };

        _repoMock.Setup(r => r.LoginAsync(request, It.IsAny<CancellationToken>()))
                 .ThrowsAsync(new Exception("Invalid credentials."));

        var act = async () => await _handler.Handle(request, CancellationToken.None);

        await act.Should().ThrowAsync<Exception>().WithMessage("Invalid credentials.");
    }
}
