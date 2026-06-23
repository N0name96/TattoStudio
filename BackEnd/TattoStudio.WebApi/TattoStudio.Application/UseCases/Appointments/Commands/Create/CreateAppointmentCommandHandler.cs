using AutoMapper;
using MediatR;
using TattoStudio.Application.Contracts;
using TattoStudio.Domain.Entities;
using TattoStudio.Application.Events;
using TattoStudio.Domain.Exceptions;

namespace TattoStudio.Application.UseCases.Appointments.Commands.Create;

/// <summary>
/// Handler que crea una nueva cita aplicando las reglas REG-03-02 y REG-03-03,
/// y publica el evento <see cref="AppointmentCreatedEvent"/> para la sincronización
/// asíncrona con Google Calendar (REG-06-03).
/// </summary>
public sealed class CreateAppointmentCommandHandler
    : IRequestHandler<CreateAppointmentCommand, CreateAppointmentResult>
{
    private readonly IAppointmentRepository _appointments;
    private readonly IMapper                _mapper;
    private readonly IPublisher             _publisher;

    /// <summary>Inyecta el repositorio de citas, el mapper y el publisher de eventos.</summary>
    public CreateAppointmentCommandHandler(
        IAppointmentRepository appointments,
        IMapper                mapper,
        IPublisher             publisher)
    {
        _appointments = appointments;
        _mapper       = mapper;
        _publisher    = publisher;
    }

    /// <summary>
    /// Valida la disponibilidad del artista (REG-03-02), construye la entidad
    /// —que aplica REG-03-03 internamente—, la persiste y publica el evento de dominio.
    /// </summary>
    public async Task<CreateAppointmentResult> Handle(
        CreateAppointmentCommand cmd,
        CancellationToken        ct)
    {
        var hasOverlap = await _appointments.HasOverlapAsync(
            cmd.ArtistId, cmd.DateTime, cmd.DurationHours, ct);

        if (hasOverlap)
            throw new ArtistOverbookedException();

        var appointment = new Appointment(
            cmd.ClientId,
            cmd.ArtistId,
            cmd.DateTime,
            cmd.DurationHours,
            cmd.DepositAmount);

        await _appointments.AddAsync(appointment, ct);
        await _appointments.SaveChangesAsync(ct);

        await _publisher.Publish(new AppointmentCreatedEvent(appointment.Id), ct);

        return _mapper.Map<CreateAppointmentResult>(appointment);
    }
}
