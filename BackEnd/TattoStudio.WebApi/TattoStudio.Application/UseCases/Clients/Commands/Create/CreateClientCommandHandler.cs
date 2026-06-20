using AutoMapper;
using MediatR;
using TattoStudio.Application.Contracts;
using TattoStudio.Domain.Entities;

namespace TattoStudio.Application.UseCases.Clients.Commands.Create;

/// <summary>
/// Handler que persiste un nuevo cliente en la base de datos.
/// </summary>
public sealed class CreateClientCommandHandler
    : IRequestHandler<CreateClientCommand, CreateClientResult>
{
    private readonly IClientRepository _clients;
    private readonly IMapper           _mapper;

    public CreateClientCommandHandler(IClientRepository clients, IMapper mapper)
    {
        _clients = clients;
        _mapper  = mapper;
    }

    public async Task<CreateClientResult> Handle(CreateClientCommand cmd, CancellationToken ct)
    {
        var client = new Client(cmd.Name, cmd.Phone, cmd.Email, cmd.BirthDate, cmd.MedicalNotes);

        await _clients.AddAsync(client, ct);
        await _clients.SaveChangesAsync(ct);

        return _mapper.Map<CreateClientResult>(client);
    }
}
