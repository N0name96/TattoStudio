namespace TattoStudio.Domain.Entities;

public class AppoinmentAuditLog
{
    public Guid Id { get; set; }
    public Guid AppoinmentId { get; set; }
    public Guid ChangedByUserId { get; set; }
    public string FieldName { get; set; } = string.Empty;
    public string? OldValue { get; set; }
    public string NewValue { get; set; } = string.Empty;
    public DateTime ChangedAt { get; set; }
}
