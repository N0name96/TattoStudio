using MediatR;
using TattoStudio.Application.DTOs.Artists;
using TattoStudio.Application.Interfaces;

namespace TattoStudio.Application.UsesCases.Commands.Artists;

public class UpdateArtistHandler : IRequestHandler<UpdateArtistCommand, ArtistDTO>
{
    private readonly IArtistRepository _artistRepository;

    public UpdateArtistHandler(IArtistRepository artistRepository)
    {
        _artistRepository = artistRepository;
    }

    public async Task<ArtistDTO> Handle(UpdateArtistCommand request, CancellationToken cancellationToken)
    {
        return await _artistRepository.UpdateAsync(request.ArtistId, request.Data, cancellationToken);
    }
}
