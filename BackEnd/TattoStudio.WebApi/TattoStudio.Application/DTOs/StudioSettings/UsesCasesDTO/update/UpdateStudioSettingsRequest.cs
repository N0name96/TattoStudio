namespace TattoStudio.Application.DTOs.StudioSettings;

public record UpdateStudioSettingsRequest
{
    public TimeOnly WorkdayStart { get; init; }
    public TimeOnly WorkdayEnd { get; init; }
}
