using MediatR;
using TattoStudio.Application.UseCases.Dashboard.Queries.GetDashboardSummary;

namespace TattoStudio.Api.Controllers;

/// <summary>
/// Controlador del Dashboard de administración. Expone GET /api/dashboard/summary (Fase 7, REG-07-03).
/// </summary>
public static class DashboardController
{
    /// <summary>Registra las rutas del controlador del Dashboard en el router de la aplicación.</summary>
    public static void MapDashboardController(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/dashboard")
                       .RequireAuthorization("AdminOnly")
                       .WithTags("Dashboard");

        group.MapGet("/summary", async (
            int               year,
            int               month,
            IMediator         mediator,
            CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetDashboardSummaryQuery(year, month), ct);
            return Results.Ok(new
            {
                totalRevenue          = result.totalRevenue,
                totalAppointments     = result.totalAppointments,
                totalCommissionsPaid  = result.totalCommissionsPaid,
                topArtistId           = result.topArtistId,
                lowStockAlertsCount   = result.lowStockAlertsCount
            });
        });
    }
}
