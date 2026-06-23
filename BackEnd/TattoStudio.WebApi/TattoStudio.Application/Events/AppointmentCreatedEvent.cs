using MediatR;

namespace TattoStudio.Application.Events;

/// <summary>
/// Evento de dominio que se publica cuando se crea una nueva cita exitosamente.
/// Permite la sincronización asíncrona con Google Calendar (REG-06-03).
/// </summary>
public record AppointmentCreatedEvent(Guid AppointmentId) : INotification;
