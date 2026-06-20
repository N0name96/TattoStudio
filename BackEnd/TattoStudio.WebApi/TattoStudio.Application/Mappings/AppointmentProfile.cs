using AutoMapper;
using TattoStudio.Application.UseCases.Appointments.Commands.ConfirmDeposit;
using TattoStudio.Application.UseCases.Appointments.Commands.Create;
using TattoStudio.Application.UseCases.Appointments.Queries.GetCalendar;
using TattoStudio.Domain.Entities;

namespace TattoStudio.Application.Mappings;

/// <summary>
/// Perfil AutoMapper para la entidad <see cref="Appointment"/>.
/// Centraliza los mappings hacia resultados de comandos y queries de la Fase 3.
/// </summary>
public sealed class AppointmentProfile : Profile
{
    /// <summary>Registra todos los mapeos de la entidad Appointment.</summary>
    public AppointmentProfile()
    {
        CreateMap<Appointment, CreateAppointmentResult>();

        CreateMap<Appointment, ConfirmDepositResult>()
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()));

        CreateMap<Appointment, AppointmentDto>()
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()));
    }
}
