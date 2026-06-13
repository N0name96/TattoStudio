using MediatR;
using TattoStudio.Application.DTOs.Artists;

namespace TattoStudio.Application.UsesCases.Queries.Artists;

public record GetArtistByIdQuery(Guid Id) : IRequest<ArtistDTO>;
