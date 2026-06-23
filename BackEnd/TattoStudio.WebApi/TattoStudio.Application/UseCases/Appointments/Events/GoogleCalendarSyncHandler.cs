using MediatR;
using Microsoft.Extensions.Logging;
using TattoStudio.Application.Contracts;
using TattoStudio.Application.Events;

namespace TattoStudio.Application.UseCases.Appointments.Events;

/// <summary>
/// Manejador del evento <see cref="AppointmentCreatedEvent"/> que sincroniza la cita
/// con Google Calendar de forma asíncrona (REG-06-03). El fallo no revierte la cita.
/// </summary>
public sealed class GoogleCalendarSyncHandler : INotificationHandler<AppointmentCreatedEvent>
{
    private readonly IGoogleCalendarService               _calendarService;
    private readonly ILogger<GoogleCalendarSyncHandler>   _logger;

    /// <summary>Inyecta el servicio de Google Calendar y el logger.</summary>
    public GoogleCalendarSyncHandler(
        IGoogleCalendarService             calendarService,
        ILogger<GoogleCalendarSyncHandler> logger)
    {
        _calendarService = calendarService;
        _logger          = logger;
    }

    /// <summary>
    /// Llama al servicio de sincronización y captura cualquier excepción sin relanzarla,
    /// garantizando que el fallo de Google Calendar nunca revierta la creación de la cita.
    /// </summary>
    public async Task Handle(AppointmentCreatedEvent notification, CancellationToken ct)
    {
        try
        {
            await _calendarService.SyncAppointmentAsync(notification.AppointmentId, ct);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "No se pudo sincronizar la cita {AppointmentId} con Google Calendar. Se continuará sin sincronización.",
                notification.AppointmentId);
        }
    }
}
