using MediatR;
using TattoStudio.Application.UseCases.Appointments.Commands.CompleteAppointment;
using TattoStudio.Application.UseCases.Appointments.Commands.ConfirmDeposit;
using TattoStudio.Application.UseCases.Appointments.Commands.Create;
using TattoStudio.Application.UseCases.Appointments.Queries.GenerateQr;
using TattoStudio.Application.UseCases.Appointments.Queries.GetById;
using TattoStudio.Application.UseCases.Appointments.Queries.GetCalendar;
using TattoStudio.Application.UseCases.Media.Commands.DeleteMedia;
using TattoStudio.Application.UseCases.Media.Commands.UploadMedia;

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

        group.MapPost("/{id:guid}/complete", async (
            Guid                       id,
            CompleteAppointmentRequest request,
            IMediator                  mediator,
            CancellationToken          ct) =>
        {
            var stockItems = request.StockItems
                .Select(s => new StockItemUsage(s.StockItemName, s.Quantity))
                .ToList();
            var result = await mediator.Send(new CompleteAppointmentCommand(id, stockItems), ct);
            return Results.Ok(new
            {
                appointmentId    = result.AppointmentId,
                status           = result.Status,
                commissionAmount = result.CommissionAmount,
                requiresRestock  = result.RequiresRestock
            });
        });

        group.MapPost("/{id:guid}/media", async (
            Guid              id,
            IFormFile         file,
            [Microsoft.AspNetCore.Mvc.FromForm] int mediaType,
            IMediator         mediator,
            CancellationToken ct) =>
        {
            using var stream = file.OpenReadStream();
            var result = await mediator.Send(
                new UploadMediaCommand(id, stream, file.FileName, file.ContentType, mediaType), ct);
            return Results.Created(
                $"/api/appointments/{id}/media/{result.Id}",
                new { id = result.Id, storageUrl = result.StorageUrl, mediaType = result.MediaType });
        })
        .DisableAntiforgery();

        group.MapDelete("/{id:guid}/media/{mediaId:guid}", async (
            Guid              id,
            Guid              mediaId,
            IMediator         mediator,
            CancellationToken ct) =>
        {
            await mediator.Send(new DeleteMediaCommand(mediaId), ct);
            return Results.NoContent();
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

/// <summary>Payload de entrada para completar una cita con ítems de stock consumidos.</summary>
internal record CompleteAppointmentRequest(IReadOnlyList<StockUsageItem> StockItems);

/// <summary>Ítem de stock consumido durante el cierre de la cita.</summary>
internal record StockUsageItem(string StockItemName, int Quantity);
