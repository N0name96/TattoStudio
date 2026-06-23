using TattoStudio.Application.Contracts;

namespace TattoStudio.Infrastructure.Services;

/// <summary>
/// Implementación no-op del servicio de Google Calendar para entornos de desarrollo y tests.
/// No realiza ninguna llamada real a la API de Google.
/// </summary>
public sealed class NullGoogleCalendarService : IGoogleCalendarService
{
    /// <summary>Simula la sincronización sin realizar ninguna llamada real a Google Calendar.</summary>
    public Task SyncAppointmentAsync(Guid appointmentId, CancellationToken ct = default)
        => Task.CompletedTask;
}
