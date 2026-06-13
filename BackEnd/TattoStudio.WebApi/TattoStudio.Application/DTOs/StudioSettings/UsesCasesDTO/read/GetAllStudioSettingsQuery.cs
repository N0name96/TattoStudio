using MediatR;
using TattoStudio.Application.DTOs.StudioSettings;

namespace TattoStudio.Application.DTOs.StudioSettings;

public record GetAllStudioSettingsQuery : IRequest<IEnumerable<StudioSettingsDTO>>;
