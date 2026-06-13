using MediatR;
using TattoStudio.Application.Interfaces;

namespace TattoStudio.Application.UsesCases.Commands.Artists;

public class DeleteArtistHandler : IRequestHandler<DeleteArtistCommand, Unit>
{
    private readonly IArtistRepository _artistRepository;

    public DeleteArtistHandler(IArtistRepository artistRepository)
    {
        _artistRepository = artistRepository;
    }

    public async Task<Unit> Handle(DeleteArtistCommand request, CancellationToken cancellationToken)
    {
        await _artistRepository.DeleteAsync(request.Id, cancellationToken);
        return Unit.Value;
    }
}
