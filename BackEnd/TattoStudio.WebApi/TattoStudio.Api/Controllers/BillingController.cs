using MediatR;
using TattoStudio.Application.UseCases.Billing.Commands.IssueInvoice;
using TattoStudio.Application.UseCases.Billing.Queries.GetVeriFactuStatus;

namespace TattoStudio.Api.Controllers;

/// <summary>
/// Controlador de facturación. Expone POST /api/billing/invoices y
/// GET /api/billing/invoices/{id}/verifactu-status (Fase 6).
/// </summary>
public static class BillingController
{
    /// <summary>Registra las rutas del controlador de facturación en el router de la aplicación.</summary>
    public static void MapBillingController(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/billing")
                       .RequireAuthorization("Staff")
                       .WithTags("Facturación");

        group.MapPost("/invoices", async (
            IssueInvoiceRequest request,
            IMediator           mediator,
            CancellationToken   ct) =>
        {
            var result = await mediator.Send(new IssueInvoiceCommand(request.PaymentId), ct);
            return Results.Created($"/api/billing/invoices/{result.Id}", result);
        });

        group.MapGet("/invoices/{id:guid}/verifactu-status", async (
            Guid              id,
            IMediator         mediator,
            CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetVeriFactuStatusQuery(id), ct);
            return Results.Ok(result);
        });
    }
}

/// <summary>Payload de entrada para emitir una factura.</summary>
internal record IssueInvoiceRequest(Guid PaymentId);
