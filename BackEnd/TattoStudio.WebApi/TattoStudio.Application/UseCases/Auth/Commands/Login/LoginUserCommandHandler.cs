using MediatR;
using TattoStudio.Application.Contracts;
using TattoStudio.Domain.Exceptions;

namespace TattoStudio.Application.UseCases.Auth.Commands.Login;

/// <summary>
/// Handler que verifica credenciales con BCrypt y genera el token JWT si son válidas.
/// </summary>
public sealed class LoginUserCommandHandler
    : IRequestHandler<LoginUserCommand, LoginUserResult>
{
    private readonly IUserRepository  _users;
    private readonly IPasswordHasher  _hasher;
    private readonly IJwtTokenService _jwt;

    public LoginUserCommandHandler(
        IUserRepository  users,
        IPasswordHasher  hasher,
        IJwtTokenService jwt)
    {
        _users  = users;
        _hasher = hasher;
        _jwt    = jwt;
    }

    public async Task<LoginUserResult> Handle(LoginUserCommand cmd, CancellationToken ct)
    {
        var user = await _users.FindByEmailAsync(cmd.Email, ct)
            ?? throw new UnauthorizedException("Credenciales inválidas");

        if (!_hasher.Verify(cmd.Password, user.PasswordHash))
            throw new UnauthorizedException("Credenciales inválidas");

        return new LoginUserResult(_jwt.GenerateToken(user));
    }
}
