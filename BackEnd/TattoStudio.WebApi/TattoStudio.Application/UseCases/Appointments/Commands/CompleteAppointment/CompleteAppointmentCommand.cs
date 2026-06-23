using MediatR;

namespace TattoStudio.Application.UseCases.Appointments.Commands.CompleteAppointment;

/// <summary>DTO de uso de material de stock durante el cierre de una cita.</summary>
public record StockItemUsage(string Name, int Quantity);

/// <summary>
/// Comando para cerrar una cita de forma transaccional (REG-05-01):
/// verifica consentimiento, registra comisión, resta stock y marca como Completada.
/// </summary>
public record CompleteAppointmentCommand(
    Guid                          AppointmentId,
    IReadOnlyList<StockItemUsage> StockItems) : IRequest<CompleteAppointmentResult>;

/// <summary>Resultado del cierre transaccional de la cita.</summary>
public record CompleteAppointmentResult(
    Guid    AppointmentId,
    string  Status,
    decimal CommissionAmount,
    bool    RequiresRestock);
