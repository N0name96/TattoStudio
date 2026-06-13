using MediatR;

namespace TattoStudio.Application.DTOs.Appoinments;

public record GetArtistAvailabilityQuery(Guid ArtistId, DateOnly Date)
    : IRequest<AvailabilityDTO>;
