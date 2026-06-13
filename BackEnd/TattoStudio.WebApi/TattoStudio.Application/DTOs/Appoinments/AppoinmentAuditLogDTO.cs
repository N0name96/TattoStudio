namespace TattoStudio.Application.DTOs.Appoinments;

public record AppoinmentAuditLogDTO
{
    public Guid Id { get; init; }
    public Guid AppoinmentId { get; init; }
    public Guid ChangedByUserId { get; init; }
    public string FieldName { get; init; } = string.Empty;
    public string? OldValue { get; init; }
    public string NewValue { get; init; } = string.Empty;
    public DateTime ChangedAt { get; init; }
}
