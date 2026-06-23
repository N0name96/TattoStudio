using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using TattoStudio.Application.Contracts;

namespace TattoStudio.Infrastructure.Persistence;

/// <summary>
/// Implementación de <see cref="IUnitOfWork"/> basada en <see cref="TattoStudioDbContext"/>.
/// En base de datos relacional (PostgreSQL) gestiona una transacción real.
/// En InMemory (tests) los métodos de transacción son no-op seguros.
/// </summary>
public sealed class UnitOfWork : IUnitOfWork
{
    private readonly TattoStudioDbContext _context;
    private IDbContextTransaction? _tx;

    /// <summary>Inyecta el DbContext principal.</summary>
    public UnitOfWork(TattoStudioDbContext context) => _context = context;

    /// <summary>Inicia una transacción si la base de datos es relacional; no-op en InMemory.</summary>
    public async Task BeginTransactionAsync(CancellationToken ct = default)
    {
        if (_context.Database.IsRelational())
            _tx = await _context.Database.BeginTransactionAsync(ct);
    }

    /// <summary>Persiste los cambios y confirma la transacción activa (si existe).</summary>
    public async Task CommitAsync(CancellationToken ct = default)
    {
        await _context.SaveChangesAsync(ct);
        if (_tx is not null)
            await _tx.CommitAsync(ct);
    }

    /// <summary>Revierte la transacción activa si existe.</summary>
    public async Task RollbackAsync(CancellationToken ct = default)
    {
        if (_tx is not null)
            await _tx.RollbackAsync(ct);
    }

    /// <summary>Libera los recursos de la transacción.</summary>
    public async ValueTask DisposeAsync()
    {
        if (_tx is not null)
            await _tx.DisposeAsync();
    }
}
