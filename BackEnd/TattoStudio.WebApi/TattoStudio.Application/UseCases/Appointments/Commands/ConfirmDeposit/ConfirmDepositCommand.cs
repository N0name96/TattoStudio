using MediatR;

namespace TattoStudio.Application.UseCases.Appointments.Commands.ConfirmDeposit;

/// <summary>
/// Comando CQRS para registrar el pago de una seña en una cita existente.
/// Actualiza HasDeposit y el Status a Confirmada (REG-03-03 post-creación).
/// </summary>
public record ConfirmDepositCommand(
    Guid    Id,
    decimal DepositAmount) : IRequest<ConfirmDepositResult>;

/// <summary>Resultado devuelto tras confirmar la seña.</summary>
public record ConfirmDepositResult(Guid Id, bool HasDeposit, string Status);
