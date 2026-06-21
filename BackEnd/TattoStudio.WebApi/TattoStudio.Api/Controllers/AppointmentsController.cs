using MediatR;
using TattoStudio.Application.UseCases.Appointments.Commands.ConfirmDeposit;
using TattoStudio.Application.UseCases.Appointments.Commands.Create;
using TattoStudio.Application.UseCases.Appointments.Queries.GenerateQr;
using TattoStudio.Application.UseCases.Appointments.Queries.GetById;
using TattoStudio.Application.UseCases.Appointments.Queries.GetCalendar;

namespace TattoStudio.Api.Controllers;

/// <summary>
/// Controlador de citas. Expone POST /api/appointments, GET /api/appointments/calendar
/// y PUT /api/appointments/{id}/confirm-deposit (spec §5, Fase 3).
/// </summary>
public static class AppointmentsController
{
    /// <summary>Registra las rutas del controlador de citas en el router de la aplicación.</summary>
    public static void MapAppointmentsController(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/appointments").RequireAuthorization("Staff").WithTags("Citas");

        group.MapPost("/", async (
            CreateAppointmentRequest request,
            IMediator                mediator,
            CancellationToken        ct) =>
        {
            var result = await mediator.Send(
                new CreateAppointmentCommand(
                    request.ClientId,
                    request.ArtistId,
                    request.DateTime,
                    request.DurationHours,
                    request.DepositAmount), ct);

            return Results.Created(
                $"/api/appointments/{result.Id}",
                new { id = result.Id, hasDeposit = result.HasDeposit });
        });

        group.MapGet("/{id:guid}", async (
            Guid              id,
            IMediator         mediator,
            CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetAppointmentByIdQuery(id), ct);
            return Results.Ok(result);
        });

        group.MapGet("/calendar", async (
            DateTime?         from,
            DateTime?         to,
            Guid?             clientId,
            Guid?             artistId,
            IMediator         mediator,
            CancellationToken ct) =>
        {
            var result = await mediator.Send(
                new GetCalendarQuery(from, to, clientId, artistId), ct);
            return Results.Ok(result);
        });

        group.MapGet("/{id:guid}/qr", async (
            Guid              id,
            IMediator         mediator,
            CancellationToken ct) =>
        {
            var result = await mediator.Send(new GenerateQrQuery(id), ct);
            return Results.Ok(new { qrBase64 = result.QrBase64, token = result.Token });
        });

        group.MapPut("/{id:guid}/confirm-deposit", async (
            Guid                  id,
            ConfirmDepositRequest request,
            IMediator             mediator,
            CancellationToken     ct) =>
        {
            var result = await mediator.Send(
                new ConfirmDepositCommand(id, request.DepositAmount), ct);

            return Results.Ok(new
            {
                id         = result.Id,
                hasDeposit = result.HasDeposit,
                status     = result.Status
            });
        });
    }
}

/// <summary>Payload de entrada para crear una cita.</summary>
internal record CreateAppointmentRequest(
    Guid     ClientId,
    Guid     ArtistId,
    DateTime DateTime,
    int      DurationHours,
    decimal  DepositAmount);

/// <summary>Payload de entrada para confirmar el pago de una seña.</summary>
internal record ConfirmDepositRequest(decimal DepositAmount);
