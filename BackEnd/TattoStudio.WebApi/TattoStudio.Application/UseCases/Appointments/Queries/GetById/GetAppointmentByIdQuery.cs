using MediatR;
using TattoStudio.Application.UseCases.Appointments.Queries.GetCalendar;

namespace TattoStudio.Application.UseCases.Appointments.Queries.GetById;

/// <summary>
/// Query CQRS para obtener el detalle completo de una cita por su identificador.
/// Lanza <see cref="Domain.Exceptions.NotFoundException"/> si no existe (HTTP 404).
/// </summary>
public record GetAppointmentByIdQuery(Guid Id) : IRequest<AppointmentDto>;
