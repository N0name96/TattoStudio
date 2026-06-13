using MediatR;

namespace TattoStudio.Application.UsesCases.Commands.Artists;

public record DeleteArtistCommand(Guid Id) : IRequest<Unit>;
