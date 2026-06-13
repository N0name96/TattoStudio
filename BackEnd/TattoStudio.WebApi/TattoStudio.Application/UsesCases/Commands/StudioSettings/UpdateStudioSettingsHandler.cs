using MediatR;
using TattoStudio.Application.DTOs.StudioSettings;
using TattoStudio.Application.Interfaces;
using TattoStudio.Domain.Exceptions;

namespace TattoStudio.Application.UsesCases.Commands.StudioSettings;

public class UpdateStudioSettingsHandler : IRequestHandler<UpdateStudioSettingsCommand, StudioSettingsDTO>
{
    private readonly IStudioSettingsRepository _repository;

    public UpdateStudioSettingsHandler(IStudioSettingsRepository repository)
        => _repository = repository;

    public async Task<StudioSettingsDTO> Handle(UpdateStudioSettingsCommand request, CancellationToken cancellationToken)
    {
        if (request.Data.WorkdayEnd <= request.Data.WorkdayStart)
            throw new StudioSettingsInvalidScheduleException();

        return await _repository.UpdateAsync(request.Id, request.Data, cancellationToken);
    }
}
