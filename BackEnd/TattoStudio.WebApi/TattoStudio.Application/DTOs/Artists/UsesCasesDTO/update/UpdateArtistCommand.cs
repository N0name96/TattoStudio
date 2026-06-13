using MediatR;

namespace TattoStudio.Application.DTOs.Artists;

public record UpdateArtistCommand(Guid ArtistId, UpdateArtistRequest Data) : IRequest<ArtistDTO>;
