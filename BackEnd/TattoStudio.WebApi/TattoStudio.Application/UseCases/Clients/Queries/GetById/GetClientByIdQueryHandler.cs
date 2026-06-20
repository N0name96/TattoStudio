using AutoMapper;
using MediatR;
using TattoStudio.Application.Contracts;
using TattoStudio.Domain.Exceptions;

namespace TattoStudio.Application.UseCases.Clients.Queries.GetById;

/// <summary>
/// Handler que recupera la ficha de un cliente o lanza <see cref="NotFoundException"/> si no existe.
/// </summary>
public sealed class GetClientByIdQueryHandler
    : IRequestHandler<GetClientByIdQuery, GetClientByIdResult>
{
    private readonly IClientRepository _clients;
    private readonly IMapper           _mapper;

    public GetClientByIdQueryHandler(IClientRepository clients, IMapper mapper)
    {
        _clients = clients;
        _mapper  = mapper;
    }

    public async Task<GetClientByIdResult> Handle(GetClientByIdQuery query, CancellationToken ct)
    {
        var client = await _clients.FindByIdAsync(query.Id, ct)
            ?? throw new NotFoundException($"Cliente con Id '{query.Id}' no encontrado.");

        return _mapper.Map<GetClientByIdResult>(client);
    }
}
