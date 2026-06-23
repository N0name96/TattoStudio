using MediatR;
using TattoStudio.Domain.Enums;

namespace TattoStudio.Application.UseCases.Payments.Commands.RegisterPayment;

/// <summary>Comando para registrar un cobro (seña o pago final) en una cita.</summary>
public record RegisterPaymentCommand(
    Guid          AppointmentId,
    decimal       Amount,
    PaymentType   Type,
    PaymentMethod Method) : IRequest<RegisterPaymentResult>;

/// <summary>Resultado del registro de pago: contiene el Id del pago creado.</summary>
public record RegisterPaymentResult(Guid Id);
