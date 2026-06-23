using Microsoft.EntityFrameworkCore;
using TattoStudio.Application.Contracts;
using TattoStudio.Domain.Entities;
using TattoStudio.Infrastructure.Persistence;

namespace TattoStudio.Infrastructure.Repositories;

/// <summary>
/// Implementación EF Core del repositorio de ítems de stock.
/// </summary>
public sealed class StockItemRepository : BaseRepository<StockItem>, IStockItemRepository
{
    /// <summary>Inyecta el DbContext principal.</summary>
    public StockItemRepository(TattoStudioDbContext context) : base(context) { }

    /// <inheritdoc/>
    public Task<StockItem?> FindByNameAsync(string name, CancellationToken ct = default) =>
        _context.StockItems.FirstOrDefaultAsync(s => s.Name == name, ct);

    /// <inheritdoc/>
    public async Task<IReadOnlyList<StockItem>> GetLowInventoryAsync(CancellationToken ct = default) =>
        await _context.StockItems
            .Where(s => s.CurrentQuantity <= s.MinThreshold)
            .ToListAsync(ct);
}
