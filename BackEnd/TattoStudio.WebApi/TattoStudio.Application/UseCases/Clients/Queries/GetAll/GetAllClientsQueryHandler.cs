using AutoMapper;
using MediatR;
using TattoStudio.Application.Contracts;

namespace TattoStudio.Application.UseCases.Clients.Queries.GetAll;

/// <summary>
/// Handler que devuelve la lista completa de clientes registrados en la base de datos.
/// </summary>
public sealed class GetAllClientsQueryHandler
    : IRequestHandler<GetAllClientsQuery, IReadOnlyList<GetAllClientsResult>>
{
    private readonly IClientRepository _clients;
    private readonly IMapper           _mapper;

    /// <summary>Inyecta el repositorio de clientes y el mapeador.</summary>
    public GetAllClientsQueryHandler(IClientRepository clients, IMapper mapper)
    {
        _clients = clients;
        _mapper  = mapper;
    }

    /// <summary>Obtiene todos los clientes y los proyecta al resultado de la query.</summary>
    public async Task<IReadOnlyList<GetAllClientsResult>> Handle(GetAllClientsQuery query, CancellationToken ct)
    {
        var clients = await _clients.GetAllAsync(ct);
        return _mapper.Map<IReadOnlyList<GetAllClientsResult>>(clients);
    }
}
