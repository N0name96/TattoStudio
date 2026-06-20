using AutoMapper;
using MediatR;
using TattoStudio.Application.Contracts;
using TattoStudio.Domain.Entities;
using TattoStudio.Domain.Exceptions;

namespace TattoStudio.Application.UseCases.Appointments.Commands.Create;

/// <summary>
/// Handler que crea una nueva cita aplicando las reglas REG-03-02 y REG-03-03.
/// </summary>
public sealed class CreateAppointmentCommandHandler
    : IRequestHandler<CreateAppointmentCommand, CreateAppointmentResult>
{
    private readonly IAppointmentRepository _appointments;
    private readonly IMapper                _mapper;

    /// <summary>Inyecta el repositorio de citas y el mapper.</summary>
    public CreateAppointmentCommandHandler(IAppointmentRepository appointments, IMapper mapper)
    {
        _appointments = appointments;
        _mapper       = mapper;
    }

    /// <summary>
    /// Valida la disponibilidad del artista (REG-03-02), construye la entidad
    /// —que aplica REG-03-03 internamente— y la persiste.
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

        return _mapper.Map<CreateAppointmentResult>(appointment);
    }
}
