using AutoMapper;
using MediatR;
using TattoStudio.Application.Contracts;
using TattoStudio.Domain.Entities;

namespace TattoStudio.Application.UseCases.Artists.Commands.Create;

/// <summary>
/// Handler que persiste un nuevo artista en la base de datos.
/// </summary>
public sealed class CreateArtistCommandHandler
    : IRequestHandler<CreateArtistCommand, CreateArtistResult>
{
    private readonly IArtistRepository _artists;
    private readonly IMapper           _mapper;

    public CreateArtistCommandHandler(IArtistRepository artists, IMapper mapper)
    {
        _artists = artists;
        _mapper  = mapper;
    }

    public async Task<CreateArtistResult> Handle(CreateArtistCommand cmd, CancellationToken ct)
    {
        var artist = new Artist(cmd.Name, cmd.Specialty, cmd.CommissionPercentage);

        await _artists.AddAsync(artist, ct);
        await _artists.SaveChangesAsync(ct);

        return _mapper.Map<CreateArtistResult>(artist);
    }
}
