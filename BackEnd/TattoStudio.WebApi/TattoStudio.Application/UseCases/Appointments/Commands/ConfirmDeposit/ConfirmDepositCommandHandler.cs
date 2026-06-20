using AutoMapper;
using MediatR;
using TattoStudio.Application.Contracts;
using TattoStudio.Domain.Exceptions;

namespace TattoStudio.Application.UseCases.Appointments.Commands.ConfirmDeposit;

/// <summary>
/// Handler que localiza la cita, registra la seña y persiste el cambio de estado.
/// </summary>
public sealed class ConfirmDepositCommandHandler
    : IRequestHandler<ConfirmDepositCommand, ConfirmDepositResult>
{
    private readonly IAppointmentRepository _appointments;
    private readonly IMapper                _mapper;

    /// <summary>Inyecta el repositorio de citas y el mapper.</summary>
    public ConfirmDepositCommandHandler(IAppointmentRepository appointments, IMapper mapper)
    {
        _appointments = appointments;
        _mapper       = mapper;
    }

    /// <summary>
    /// Busca la cita, llama a <see cref="Domain.Entities.Appointment.ConfirmDeposit"/>
    /// y guarda los cambios. Lanza <see cref="NotFoundException"/> si no existe (HTTP 404).
    /// </summary>
    public async Task<ConfirmDepositResult> Handle(
        ConfirmDepositCommand cmd,
        CancellationToken     ct)
    {
        var appointment = await _appointments.FindByIdAsync(cmd.Id, ct)
            ?? throw new NotFoundException($"Cita {cmd.Id} no encontrada.");

        appointment.ConfirmDeposit(cmd.DepositAmount);
        await _appointments.SaveChangesAsync(ct);

        return _mapper.Map<ConfirmDepositResult>(appointment);
    }
}
