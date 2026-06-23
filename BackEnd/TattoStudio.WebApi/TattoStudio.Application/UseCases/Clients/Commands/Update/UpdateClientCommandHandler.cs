using AutoMapper;
using MediatR;
using TattoStudio.Application.Contracts;
using TattoStudio.Domain.Exceptions;

namespace TattoStudio.Application.UseCases.Clients.Commands.Update;

/// <summary>
/// Handler que actualiza los datos de un cliente existente o lanza <see cref="NotFoundException"/> si no existe.
/// </summary>
public sealed class UpdateClientCommandHandler
    : IRequestHandler<UpdateClientCommand, UpdateClientResult>
{
    private readonly IClientRepository _clients;
    private readonly IMapper           _mapper;

    /// <summary>Inyecta el repositorio de clientes y el mapeador.</summary>
    public UpdateClientCommandHandler(IClientRepository clients, IMapper mapper)
    {
        _clients = clients;
        _mapper  = mapper;
    }

    /// <summary>Busca el cliente, aplica los cambios y persiste el resultado.</summary>
    public async Task<UpdateClientResult> Handle(UpdateClientCommand cmd, CancellationToken ct)
    {
        var client = await _clients.FindByIdAsync(cmd.Id, ct)
            ?? throw new NotFoundException($"Cliente con Id '{cmd.Id}' no encontrado.");

        client.Update(cmd.Name, cmd.Phone, cmd.Email, cmd.BirthDate, cmd.MedicalNotes);

        await _clients.SaveChangesAsync(ct);

        return _mapper.Map<UpdateClientResult>(client);
    }
}
