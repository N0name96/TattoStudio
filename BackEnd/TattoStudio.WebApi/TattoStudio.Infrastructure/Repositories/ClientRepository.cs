using Microsoft.EntityFrameworkCore;
using TattoStudio.Application.Contracts;
using TattoStudio.Domain.Entities;
using TattoStudio.Infrastructure.Persistence;

namespace TattoStudio.Infrastructure.Repositories;

/// <summary>
/// Implementación EF Core del repositorio de clientes.
/// </summary>
public sealed class ClientRepository : IClientRepository
{
    private readonly TattoStudioDbContext _context;

    public ClientRepository(TattoStudioDbContext context) => _context = context;

    public async Task AddAsync(Client client, CancellationToken ct = default) =>
        await _context.Clients.AddAsync(client, ct);

    public Task<Client?> FindByIdAsync(Guid id, CancellationToken ct = default) =>
        _context.Clients.FirstOrDefaultAsync(c => c.Id == id, ct);

    public Task SaveChangesAsync(CancellationToken ct = default) =>
        _context.SaveChangesAsync(ct);
}
