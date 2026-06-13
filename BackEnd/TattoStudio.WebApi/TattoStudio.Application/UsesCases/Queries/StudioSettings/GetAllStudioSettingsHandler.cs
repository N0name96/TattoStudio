using MediatR;
using TattoStudio.Application.DTOs.StudioSettings;
using TattoStudio.Application.Interfaces;

namespace TattoStudio.Application.UsesCases.Queries.StudioSettings;

public class GetAllStudioSettingsHandler : IRequestHandler<GetAllStudioSettingsQuery, IEnumerable<StudioSettingsDTO>>
{
    private readonly IStudioSettingsRepository _repository;

    public GetAllStudioSettingsHandler(IStudioSettingsRepository repository)
        => _repository = repository;

    public async Task<IEnumerable<StudioSettingsDTO>> Handle(GetAllStudioSettingsQuery request, CancellationToken cancellationToken)
        => await _repository.GetAllAsync(cancellationToken);
}
