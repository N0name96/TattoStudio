namespace TattoStudio.Application.DTOs.Appoinments;

public record TimeSlotDTO(TimeOnly Start, TimeOnly End);

public record AvailabilityDTO
{
    public Guid ArtistId { get; init; }
    public DateOnly Date { get; init; }
    public TimeOnly? StudioStart { get; init; }
    public TimeOnly? StudioEnd { get; init; }
    public IEnumerable<TimeSlotDTO> AvailableSlots { get; init; } = [];
}
