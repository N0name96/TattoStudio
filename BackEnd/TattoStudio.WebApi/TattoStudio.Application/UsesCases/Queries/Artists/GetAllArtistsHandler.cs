using MediatR;
using TattoStudio.Application.DTOs.Artists;
using TattoStudio.Application.Interfaces;

namespace TattoStudio.Application.UsesCases.Queries.Artists;

public class GetAllArtistsHandler : IRequestHandler<GetAllArtistsQuery, IEnumerable<ArtistDTO>>
{
    private readonly IArtistRepository _artistRepository;

    public GetAllArtistsHandler(IArtistRepository artistRepository)
    {
        _artistRepository = artistRepository;
    }

    public async Task<IEnumerable<ArtistDTO>> Handle(GetAllArtistsQuery request, CancellationToken cancellationToken)
    {
        return await _artistRepository.GetAllAsync(request.IncludeInactive, cancellationToken);
    }
}
