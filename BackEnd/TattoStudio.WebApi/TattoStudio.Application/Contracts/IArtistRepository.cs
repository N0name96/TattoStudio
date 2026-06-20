using TattoStudio.Domain.Entities;

namespace TattoStudio.Application.Contracts;

/// <summary>
/// Contrato de persistencia para la entidad <see cref="Artist"/>.
/// </summary>
public interface IArtistRepository
{
    Task AddAsync(Artist artist, CancellationToken ct = default);
    Task<IReadOnlyList<Artist>> GetAllAsync(CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
