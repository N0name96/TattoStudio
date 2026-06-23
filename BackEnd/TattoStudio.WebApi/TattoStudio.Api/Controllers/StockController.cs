using MediatR;
using TattoStudio.Application.UseCases.Stock.Commands.CreateStockItem;
using TattoStudio.Application.UseCases.Stock.Queries.GetLowInventory;

namespace TattoStudio.Api.Controllers;

/// <summary>
/// Controlador de inventario. Expone POST /api/stock y GET /api/stock/low-inventory (Fase 5).
/// </summary>
public static class StockController
{
    /// <summary>Registra las rutas del controlador de stock en el router de la aplicación.</summary>
    public static void MapStockController(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/stock")
                       .RequireAuthorization("Staff")
                       .WithTags("Stock");

        group.MapPost("/", async (
            CreateStockItemRequest request,
            IMediator              mediator,
            CancellationToken      ct) =>
        {
            var result = await mediator.Send(
                new CreateStockItemCommand(
                    request.Name,
                    request.CurrentQuantity,
                    request.MinThreshold), ct);

            return Results.Created($"/api/stock/{result.Id}", new { id = result.Id });
        });

        group.MapGet("/low-inventory", async (
            IMediator         mediator,
            CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetLowInventoryQuery(), ct);
            return Results.Ok(result);
        });
    }
}

/// <summary>Payload de entrada para crear un ítem de stock.</summary>
internal record CreateStockItemRequest(string Name, int CurrentQuantity, int MinThreshold);
