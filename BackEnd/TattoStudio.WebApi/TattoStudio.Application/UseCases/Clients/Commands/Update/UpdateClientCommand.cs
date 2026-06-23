using MediatR;

namespace TattoStudio.Application.UseCases.Clients.Commands.Update;

/// <summary>
/// Comando CQRS para actualizar los datos de un cliente existente.
/// </summary>
public record UpdateClientCommand(
    Guid     Id,
    string   Name,
    string   Phone,
    string   Email,
    DateOnly BirthDate,
    string?  MedicalNotes) : IRequest<UpdateClientResult>;

/// <summary>Resultado con los datos actualizados del cliente.</summary>
public record UpdateClientResult(
    Guid     Id,
    string   Name,
    string   Phone,
    string   Email,
    DateOnly BirthDate,
    string?  MedicalNotes);
