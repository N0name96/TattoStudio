using TattoStudio.Application.DTOs.Artists;

namespace TattoStudio.Application.Interfaces;

public interface IArtistRepository
{
    Task<ArtistDTO> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<ArtistDTO>> GetAllAsync(bool includeInactive, CancellationToken cancellationToken);
    Task<ArtistDTO> CreateAsync(CreateArtistCommand request, CancellationToken cancellationToken);
    Task<ArtistDTO> UpdateAsync(Guid artistId, UpdateArtistRequest request, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
