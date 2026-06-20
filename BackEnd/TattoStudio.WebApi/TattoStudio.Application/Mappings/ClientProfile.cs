using AutoMapper;
using TattoStudio.Application.UseCases.Clients.Commands.Create;
using TattoStudio.Application.UseCases.Clients.Queries.GetById;
using TattoStudio.Domain.Entities;

namespace TattoStudio.Application.Mappings;

/// <summary>
/// Perfil AutoMapper para la entidad <see cref="Client"/>.
/// Centraliza los mappings hacia resultados de comandos y queries.
/// </summary>
public sealed class ClientProfile : Profile
{
    public ClientProfile()
    {
        CreateMap<Client, CreateClientResult>();
        CreateMap<Client, GetClientByIdResult>();
    }
}
