using TattoStudio.Domain.Enums;

namespace TattoStudio.Application.DTOs.Appoinments;

public record ChangeAppoinmentStatusRequest
{
    public AppoinmentStatus Status { get; init; }
    public string? CancellationReason { get; init; }
}
