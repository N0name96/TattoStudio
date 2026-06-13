using MediatR;
using TattoStudio.Application.DTOs.StudioSettings;
using TattoStudio.Application.Interfaces;
using TattoStudio.Domain.Exceptions;

namespace TattoStudio.Application.UsesCases.Commands.StudioSettings;

public class CreateStudioSettingsHandler : IRequestHandler<CreateStudioSettingsCommand, StudioSettingsDTO>
{
    private readonly IStudioSettingsRepository _repository;

    public CreateStudioSettingsHandler(IStudioSettingsRepository repository)
        => _repository = repository;

    public async Task<StudioSettingsDTO> Handle(CreateStudioSettingsCommand request, CancellationToken cancellationToken)
    {
        if (request.WorkdayEnd <= request.WorkdayStart)
            throw new StudioSettingsInvalidScheduleException();

        return await _repository.CreateAsync(request, cancellationToken);
    }
}
