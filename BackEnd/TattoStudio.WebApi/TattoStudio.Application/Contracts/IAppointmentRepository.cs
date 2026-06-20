using TattoStudio.Domain.Entities;

namespace TattoStudio.Application.Contracts;

/// <summary>
/// Contrato de persistencia para la entidad <see cref="Appointment"/>.
/// </summary>
public interface IAppointmentRepository
{
    /// <summary>Persiste una nueva cita en el contexto (sin SaveChanges).</summary>
    Task AddAsync(Appointment appointment, CancellationToken ct = default);

    /// <summary>Busca una cita por su identificador. Devuelve null si no existe.</summary>
    Task<Appointment?> FindByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Comprueba si el artista ya tiene una cita activa (Pendiente o Confirmada)
    /// cuyo rango horario colisiona con el solicitado (REG-03-02).
    /// </summary>
    Task<bool> HasOverlapAsync(
        Guid              artistId,
        DateTime          dateTime,
        int               durationHours,
        CancellationToken ct = default);

    /// <summary>
    /// Devuelve citas aplicando todos los filtros de forma opcional.
    /// Si <paramref name="from"/> es null no se aplica límite inferior de fecha.
    /// Si <paramref name="to"/> es null no se aplica límite superior de fecha.
    /// </summary>
    Task<IReadOnlyList<Appointment>> GetByDateRangeAsync(
        DateTime?         from      = null,
        DateTime?         to        = null,
        Guid?             clientId  = null,
        Guid?             artistId  = null,
        CancellationToken ct        = default);

    /// <summary>Persiste los cambios pendientes en el contexto.</summary>
    Task SaveChangesAsync(CancellationToken ct = default);
}
