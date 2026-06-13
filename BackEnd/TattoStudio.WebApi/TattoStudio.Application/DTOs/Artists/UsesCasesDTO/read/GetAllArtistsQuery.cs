using MediatR;
using TattoStudio.Application.DTOs.Artists;

namespace TattoStudio.Application.UsesCases.Queries.Artists;

public record GetAllArtistsQuery : IRequest<IEnumerable<ArtistDTO>>
{
    public bool IncludeInactive { get; init; } = false;
}
