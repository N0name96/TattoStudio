namespace TattoStudio.Application.DTOs.StudioSettings;

public record StudioSettingsDTO
{
    public Guid Id { get; init; }
    public TimeOnly WorkdayStart { get; init; }
    public TimeOnly WorkdayEnd { get; init; }
}
