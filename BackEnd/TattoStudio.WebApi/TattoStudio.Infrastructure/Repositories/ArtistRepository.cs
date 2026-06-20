using Microsoft.EntityFrameworkCore;
using TattoStudio.Application.Contracts;
using TattoStudio.Domain.Entities;
using TattoStudio.Infrastructure.Persistence;

namespace TattoStudio.Infrastructure.Repositories;

/// <summary>
/// Implementación EF Core del repositorio de artistas.
/// </summary>
public sealed class ArtistRepository : IArtistRepository
{
    private readonly TattoStudioDbContext _context;

    public ArtistRepository(TattoStudioDbContext context) => _context = context;

    public async Task AddAsync(Artist artist, CancellationToken ct = default) =>
        await _context.Artists.AddAsync(artist, ct);

    public async Task<IReadOnlyList<Artist>> GetAllAsync(CancellationToken ct = default) =>
        await _context.Artists.ToListAsync(ct);

    public Task SaveChangesAsync(CancellationToken ct = default) =>
        _context.SaveChangesAsync(ct);
}
