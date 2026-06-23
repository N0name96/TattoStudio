using MediatR;
using TattoStudio.Application.UseCases.Payments.Commands.RegisterPayment;
using TattoStudio.Domain.Enums;

namespace TattoStudio.Api.Controllers;

/// <summary>
/// Controlador de pagos. Expone POST /api/payments para registrar cobros de citas (Fase 5).
/// </summary>
public static class PaymentsController
{
    /// <summary>Registra las rutas del controlador de pagos en el router de la aplicación.</summary>
    public static void MapPaymentsController(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/payments")
                       .RequireAuthorization("Staff")
                       .WithTags("Pagos");

        group.MapPost("/", async (
            RegisterPaymentRequest request,
            IMediator              mediator,
            CancellationToken      ct) =>
        {
            var result = await mediator.Send(
                new RegisterPaymentCommand(
                    request.AppointmentId,
                    request.Amount,
                    (PaymentType)request.Type,
                    (PaymentMethod)request.Method), ct);

            return Results.Created($"/api/payments/{result.Id}", new { id = result.Id });
        });
    }
}

/// <summary>Payload de entrada para registrar un pago en una cita.</summary>
internal record RegisterPaymentRequest(Guid AppointmentId, decimal Amount, int Type, int Method);
