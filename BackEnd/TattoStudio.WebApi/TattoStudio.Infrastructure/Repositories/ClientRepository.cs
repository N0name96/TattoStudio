using Microsoft.EntityFrameworkCore;
using TattoStudio.Application.Contracts;
using TattoStudio.Domain.Entities;
using TattoStudio.Infrastructure.Persistence;

namespace TattoStudio.Infrastructure.Repositories;

/// <summary>
/// Implementación EF Core del repositorio de clientes.
/// </summary>
public sealed class ClientRepository : BaseRepository<Client>, IClientRepository
{
    /// <summary>Inyecta el DbContext principal.</summary>
    public ClientRepository(TattoStudioDbContext context) : base(context) { }

    /// <inheritdoc/>
    public Task<Client?> FindByIdAsync(Guid id, CancellationToken ct = default) =>
        _context.Clients.FirstOrDefaultAsync(c => c.Id == id, ct);

    /// <inheritdoc/>
    public async Task<IReadOnlyList<Client>> GetAllAsync(CancellationToken ct = default) =>
        await _context.Clients.ToListAsync(ct);
}
