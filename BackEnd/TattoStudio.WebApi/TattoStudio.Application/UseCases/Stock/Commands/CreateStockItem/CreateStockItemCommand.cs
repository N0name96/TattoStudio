using MediatR;

namespace TattoStudio.Application.UseCases.Stock.Commands.CreateStockItem;

/// <summary>Comando para crear un nuevo ítem en el inventario de materiales.</summary>
public record CreateStockItemCommand(
    string Name,
    int    CurrentQuantity,
    int    MinThreshold) : IRequest<CreateStockItemResult>;

/// <summary>Resultado de la creación del ítem de stock: contiene el Id generado.</summary>
public record CreateStockItemResult(Guid Id);
