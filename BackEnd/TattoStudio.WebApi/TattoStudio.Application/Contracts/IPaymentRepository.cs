using TattoStudio.Domain.Entities;

namespace TattoStudio.Application.Contracts;

/// <summary>
/// Contrato de persistencia para la entidad <see cref="Payment"/>.
/// </summary>
public interface IPaymentRepository
{
    /// <summary>Persiste un nuevo pago en el contexto (sin SaveChanges).</summary>
    Task AddAsync(Payment payment, CancellationToken ct = default);

    /// <summary>Devuelve todos los pagos asociados a la cita indicada.</summary>
    Task<IReadOnlyList<Payment>> GetByAppointmentIdAsync(Guid appointmentId, CancellationToken ct = default);

    /// <summary>Busca un pago por su identificador único.</summary>
    Task<Payment?> FindByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>Persiste los cambios pendientes en el contexto.</summary>
    Task SaveChangesAsync(CancellationToken ct = default);
}
