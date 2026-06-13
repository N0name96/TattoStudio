using TattoStudio.Application.DTOs.StudioSettings;

namespace TattoStudio.Application.Interfaces;

public interface IStudioSettingsRepository
{
    Task<IEnumerable<StudioSettingsDTO>> GetAllAsync(CancellationToken cancellationToken);
    Task<StudioSettingsDTO?> GetFirstOrDefaultAsync(CancellationToken cancellationToken);
    Task<StudioSettingsDTO> CreateAsync(CreateStudioSettingsCommand request, CancellationToken cancellationToken);
    Task<StudioSettingsDTO> UpdateAsync(Guid id, UpdateStudioSettingsRequest request, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
