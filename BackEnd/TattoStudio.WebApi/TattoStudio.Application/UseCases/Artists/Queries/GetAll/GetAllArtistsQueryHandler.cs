using AutoMapper;
using MediatR;
using TattoStudio.Application.Contracts;

namespace TattoStudio.Application.UseCases.Artists.Queries.GetAll;

/// <summary>
/// Handler que devuelve la lista completa de artistas mapeada a <see cref="ArtistDto"/>.
/// </summary>
public sealed class GetAllArtistsQueryHandler
    : IRequestHandler<GetAllArtistsQuery, IReadOnlyList<ArtistDto>>
{
    private readonly IArtistRepository _artists;
    private readonly IMapper           _mapper;

    public GetAllArtistsQueryHandler(IArtistRepository artists, IMapper mapper)
    {
        _artists = artists;
        _mapper  = mapper;
    }

    public async Task<IReadOnlyList<ArtistDto>> Handle(GetAllArtistsQuery query, CancellationToken ct)
    {
        var artists = await _artists.GetAllAsync(ct);
        return _mapper.Map<IReadOnlyList<ArtistDto>>(artists);
    }
}
