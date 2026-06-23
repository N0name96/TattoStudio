using MediatR;

namespace TattoStudio.Application.UseCases.Stock.Queries.GetLowInventory;

/// <summary>Query para obtener los ítems de stock cuya cantidad está por debajo del umbral mínimo (REG-05-02).</summary>
public record GetLowInventoryQuery() : IRequest<IReadOnlyList<StockItemDto>>;

/// <summary>DTO de un ítem de stock devuelto por la consulta de bajo inventario.</summary>
public record StockItemDto(
    Guid   Id,
    string Name,
    int    CurrentQuantity,
    int    MinThreshold,
    bool   RequiresRestock);
