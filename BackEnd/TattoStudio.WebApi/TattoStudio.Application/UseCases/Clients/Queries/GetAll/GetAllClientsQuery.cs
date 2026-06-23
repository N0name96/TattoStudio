using MediatR;

namespace TattoStudio.Application.UseCases.Clients.Queries.GetAll;

/// <summary>
/// Query CQRS para obtener la lista completa de clientes registrados.
/// </summary>
public record GetAllClientsQuery() : IRequest<IReadOnlyList<GetAllClientsResult>>;

/// <summary>Resultado con los datos públicos de cada cliente en la lista.</summary>
public record GetAllClientsResult(
    Guid     Id,
    string   Name,
    string   Phone,
    string   Email,
    DateOnly BirthDate,
    string?  MedicalNotes);
