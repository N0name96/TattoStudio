using MediatR;

namespace TattoStudio.Application.UseCases.Auth.Commands.Login;

/// <summary>
/// Comando CQRS para autenticar un usuario y obtener un token JWT (spec REG-01-02, REG-01-03).
/// </summary>
public record LoginUserCommand(
    string Email,
    string Password) : IRequest<LoginUserResult>;

public record LoginUserResult(string Token);
