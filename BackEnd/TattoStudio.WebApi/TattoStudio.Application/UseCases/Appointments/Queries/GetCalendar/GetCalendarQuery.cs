using MediatR;

namespace TattoStudio.Application.UseCases.Appointments.Queries.GetCalendar;

/// <summary>
/// Query CQRS para obtener el calendario de citas con todos los filtros opcionales.
/// Cualquier combinación de From, To, ClientId y ArtistId es válida; si se omiten
/// todos los filtros se devuelven todas las citas.
/// Corresponde a GET /api/appointments/calendar[?from=][&amp;to=][&amp;clientId=][&amp;artistId=].
/// </summary>
public record GetCalendarQuery(
    DateTime? From     = null,
    DateTime? To       = null,
    Guid?     ClientId = null,
    Guid?     ArtistId = null) : IRequest<IReadOnlyList<AppointmentDto>>;

/// <summary>DTO de presentación de una cita en el calendario.</summary>
public record AppointmentDto(
    Guid     Id,
    Guid     ClientId,
    Guid     ArtistId,
    DateTime DateTime,
    int      DurationHours,
    string   Status,
    bool     HasDeposit,
    decimal  DepositAmount);
