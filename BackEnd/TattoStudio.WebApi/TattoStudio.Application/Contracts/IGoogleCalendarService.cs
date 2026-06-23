namespace TattoStudio.Application.Contracts;

/// <summary>
/// Contrato del servicio de sincronización con Google Calendar.
/// </summary>
public interface IGoogleCalendarService
{
    /// <summary>
    /// Sincroniza la cita indicada con Google Calendar de forma asíncrona.
    /// </summary>
    Task SyncAppointmentAsync(Guid appointmentId, CancellationToken ct = default);
}
