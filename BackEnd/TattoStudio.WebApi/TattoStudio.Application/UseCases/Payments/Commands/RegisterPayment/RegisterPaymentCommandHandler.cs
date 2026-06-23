using AutoMapper;
using MediatR;
using TattoStudio.Application.Contracts;
using TattoStudio.Domain.Entities;
using TattoStudio.Domain.Exceptions;

namespace TattoStudio.Application.UseCases.Payments.Commands.RegisterPayment;

/// <summary>
/// Handler del comando <see cref="RegisterPaymentCommand"/>.
/// Valida la existencia de la cita, crea el pago y lo persiste.
/// </summary>
public sealed class RegisterPaymentCommandHandler : IRequestHandler<RegisterPaymentCommand, RegisterPaymentResult>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IPaymentRepository     _paymentRepository;
    private readonly IMapper                _mapper;

    /// <summary>Inyecta los repositorios y el mapper necesarios.</summary>
    public RegisterPaymentCommandHandler(
        IAppointmentRepository appointmentRepository,
        IPaymentRepository     paymentRepository,
        IMapper                mapper)
    {
        _appointmentRepository = appointmentRepository;
        _paymentRepository     = paymentRepository;
        _mapper                = mapper;
    }

    /// <summary>Ejecuta el registro del pago para la cita indicada.</summary>
    public async Task<RegisterPaymentResult> Handle(RegisterPaymentCommand command, CancellationToken ct)
    {
        var appointment = await _appointmentRepository.FindByIdAsync(command.AppointmentId, ct)
            ?? throw new NotFoundException($"Cita '{command.AppointmentId}' no encontrada.");

        var payment = new Payment(
            appointment.Id,
            command.Amount,
            command.Type,
            command.Method);

        await _paymentRepository.AddAsync(payment, ct);
        await _paymentRepository.SaveChangesAsync(ct);

        return _mapper.Map<RegisterPaymentResult>(payment);
    }
}
