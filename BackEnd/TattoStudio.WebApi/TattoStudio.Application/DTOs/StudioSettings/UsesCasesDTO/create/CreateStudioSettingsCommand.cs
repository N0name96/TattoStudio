using MediatR;
using TattoStudio.Application.DTOs.StudioSettings;

namespace TattoStudio.Application.DTOs.StudioSettings;

public record CreateStudioSettingsCommand : IRequest<StudioSettingsDTO>
{
    public TimeOnly WorkdayStart { get; init; }
    public TimeOnly WorkdayEnd { get; init; }
}
