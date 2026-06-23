using TattoStudio.Domain.Entities;

namespace TattoStudio.Application.Contracts;

/// <summary>
/// Contrato de persistencia para la entidad <see cref="StockItem"/>.
/// </summary>
public interface IStockItemRepository
{
    /// <summary>Persiste un nuevo ítem de stock en el contexto (sin SaveChanges).</summary>
    Task AddAsync(StockItem item, CancellationToken ct = default);

    /// <summary>Busca un ítem por nombre. Devuelve null si no existe.</summary>
    Task<StockItem?> FindByNameAsync(string name, CancellationToken ct = default);

    /// <summary>Devuelve todos los ítems cuyo stock está en nivel bajo (REG-05-02).</summary>
    Task<IReadOnlyList<StockItem>> GetLowInventoryAsync(CancellationToken ct = default);

    /// <summary>Persiste los cambios pendientes en el contexto.</summary>
    Task SaveChangesAsync(CancellationToken ct = default);
}
