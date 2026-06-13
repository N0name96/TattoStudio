using MediatR;
using TattoStudio.Application.DTOs.Artists;
using TattoStudio.Application.Interfaces;

namespace TattoStudio.Application.UsesCases.Queries.Artists;

public class GetArtistByIdHandler : IRequestHandler<GetArtistByIdQuery, ArtistDTO>
{
    private readonly IArtistRepository _artistRepository;

    public GetArtistByIdHandler(IArtistRepository artistRepository)
    {
        _artistRepository = artistRepository;
    }

    public async Task<ArtistDTO> Handle(GetArtistByIdQuery request, CancellationToken cancellationToken)
    {
        return await _artistRepository.GetByIdAsync(request.Id, cancellationToken);
    }
}
