using MediatR;

namespace TattoStudio.Application.UseCases.Auth.Commands.Register;

/// <summary>
/// Comando CQRS para registrar un nuevo usuario en el sistema (spec REG-01-01).
/// </summary>
public record RegisterUserCommand(
    string Email,
    string Password,
    string Name,
    int    Role) : IRequest<RegisterUserResult>;

public record RegisterUserResult(Guid Id);
