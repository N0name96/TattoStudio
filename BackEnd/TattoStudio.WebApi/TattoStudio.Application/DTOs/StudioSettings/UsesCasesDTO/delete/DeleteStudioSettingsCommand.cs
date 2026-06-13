using MediatR;

namespace TattoStudio.Application.DTOs.StudioSettings;

public record DeleteStudioSettingsCommand(Guid Id) : IRequest<MediatR.Unit>;
