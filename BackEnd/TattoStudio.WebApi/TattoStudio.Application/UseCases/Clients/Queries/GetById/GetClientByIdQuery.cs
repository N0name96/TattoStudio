using MediatR;

namespace TattoStudio.Application.UseCases.Clients.Queries.GetById;

/// <summary>
/// Query CQRS para obtener la ficha completa de un cliente por su identificador.
/// </summary>
public record GetClientByIdQuery(Guid Id) : IRequest<GetClientByIdResult>;

/// <summary>Resultado con los datos públicos del cliente.</summary>
public record GetClientByIdResult(
    Guid     Id,
    string   Name,
    string   Phone,
    string   Email,
    DateOnly BirthDate,
    string?  MedicalNotes);
