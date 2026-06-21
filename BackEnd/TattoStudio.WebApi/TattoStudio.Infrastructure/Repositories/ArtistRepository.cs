using Microsoft.EntityFrameworkCore;
using TattoStudio.Application.Contracts;
using TattoStudio.Domain.Entities;
using TattoStudio.Infrastructure.Persistence;

namespace TattoStudio.Infrastructure.Repositories;

/// <summary>
/// Implementación EF Core del repositorio de artistas.
/// </summary>
public sealed class ArtistRepository : BaseRepository<Artist>, IArtistRepository
{
    /// <summary>Inyecta el DbContext principal.</summary>
    public ArtistRepository(TattoStudioDbContext context) : base(context) { }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<Artist>> GetAllAsync(CancellationToken ct = default) =>
        await _context.Artists.ToListAsync(ct);
}
