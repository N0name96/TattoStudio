using MediatR;
using TattoStudio.Application.DTOs.Artists;
using TattoStudio.Application.Interfaces;

namespace TattoStudio.Application.UsesCases.Commands.Artists;

public class CreateArtistHandler : IRequestHandler<CreateArtistCommand, ArtistDTO>
{
    private readonly IArtistRepository _artistRepository;

    public CreateArtistHandler(IArtistRepository artistRepository)
    {
        _artistRepository = artistRepository;
    }

    public async Task<ArtistDTO> Handle(CreateArtistCommand request, CancellationToken cancellationToken)
    {
        return await _artistRepository.CreateAsync(request, cancellationToken);
    }
}
