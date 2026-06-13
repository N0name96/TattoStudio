namespace TattoStudio.Domain.Entities;

public class StudioSettings
{
    public Guid Id { get; set; }
    public TimeOnly WorkdayStart { get; set; }
    public TimeOnly WorkdayEnd { get; set; }
}
