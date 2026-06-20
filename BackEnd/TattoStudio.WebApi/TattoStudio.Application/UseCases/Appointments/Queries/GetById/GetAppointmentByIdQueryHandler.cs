using AutoMapper;
using MediatR;
using TattoStudio.Application.Contracts;
using TattoStudio.Application.UseCases.Appointments.Queries.GetCalendar;
using TattoStudio.Domain.Exceptions;

namespace TattoStudio.Application.UseCases.Appointments.Queries.GetById;

/// <summary>
/// Handler que busca una cita por Id y la proyecta a <see cref="AppointmentDto"/>.
/// Lanza <see cref="NotFoundException"/> si la cita no existe (HTTP 404).
/// </summary>
public sealed class GetAppointmentByIdQueryHandler
    : IRequestHandler<GetAppointmentByIdQuery, AppointmentDto>
{
    private readonly IAppointmentRepository _appointments;
    private readonly IMapper                _mapper;

    /// <summary>Inyecta el repositorio de citas y el mapper.</summary>
    public GetAppointmentByIdQueryHandler(IAppointmentRepository appointments, IMapper mapper)
    {
        _appointments = appointments;
        _mapper       = mapper;
    }

    /// <summary>Localiza la cita o lanza NotFoundException si no existe.</summary>
    public async Task<AppointmentDto> Handle(
        GetAppointmentByIdQuery query,
        CancellationToken       ct)
    {
        var appointment = await _appointments.FindByIdAsync(query.Id, ct)
            ?? throw new NotFoundException($"Cita {query.Id} no encontrada.");

        return _mapper.Map<AppointmentDto>(appointment);
    }
}
