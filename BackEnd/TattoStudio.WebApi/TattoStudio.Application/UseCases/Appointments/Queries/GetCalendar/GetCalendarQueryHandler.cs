using AutoMapper;
using MediatR;
using TattoStudio.Application.Contracts;

namespace TattoStudio.Application.UseCases.Appointments.Queries.GetCalendar;

/// <summary>
/// Handler que devuelve el listado de citas en el rango de fechas solicitado
/// mapeado a <see cref="AppointmentDto"/>.
/// </summary>
public sealed class GetCalendarQueryHandler
    : IRequestHandler<GetCalendarQuery, IReadOnlyList<AppointmentDto>>
{
    private readonly IAppointmentRepository _appointments;
    private readonly IMapper                _mapper;

    /// <summary>Inyecta el repositorio de citas y el mapper.</summary>
    public GetCalendarQueryHandler(IAppointmentRepository appointments, IMapper mapper)
    {
        _appointments = appointments;
        _mapper       = mapper;
    }

    /// <summary>Consulta las citas dentro del rango y las proyecta a DTOs.</summary>
    public async Task<IReadOnlyList<AppointmentDto>> Handle(
        GetCalendarQuery  query,
        CancellationToken ct)
    {
        var appointments = await _appointments.GetByDateRangeAsync(
            query.From, query.To, query.ClientId, query.ArtistId, ct);
        return _mapper.Map<IReadOnlyList<AppointmentDto>>(appointments);
    }
}
