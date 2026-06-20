using MediatR;

namespace TattoStudio.Application.UseCases.Clients.Commands.Create;

/// <summary>
/// Comando CQRS para registrar un nuevo cliente en el sistema (Fase 2).
/// </summary>
public record CreateClientCommand(
    string  Name,
    string  Phone,
    string  Email,
    DateOnly BirthDate,
    string?  MedicalNotes) : IRequest<CreateClientResult>;

/// <summary>Resultado del comando con el identificador del cliente creado.</summary>
public record CreateClientResult(Guid Id);
