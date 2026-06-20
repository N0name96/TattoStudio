using AutoMapper;
using TattoStudio.Application.UseCases.Artists.Commands.Create;
using TattoStudio.Application.UseCases.Artists.Queries.GetAll;
using TattoStudio.Domain.Entities;

namespace TattoStudio.Application.Mappings;

/// <summary>
/// Perfil AutoMapper para la entidad <see cref="Artist"/>.
/// Centraliza los mappings hacia resultados de comandos y queries.
/// </summary>
public sealed class ArtistProfile : Profile
{
    public ArtistProfile()
    {
        CreateMap<Artist, CreateArtistResult>();
        CreateMap<Artist, ArtistDto>();
    }
}
