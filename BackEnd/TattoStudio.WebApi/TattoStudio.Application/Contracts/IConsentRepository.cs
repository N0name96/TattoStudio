using TattoStudio.Domain.Entities;

namespace TattoStudio.Application.Contracts;

/// <summary>
/// Contrato de persistencia para la entidad <see cref="Consent"/>.
/// </summary>
public interface IConsentRepository
{
    /// <summary>Busca un consentimiento por el identificador de la cita asociada. Devuelve null si no existe.</summary>
    Task<Consent?> GetByAppointmentIdAsync(Guid appointmentId, CancellationToken ct = default);

    /// <summary>Agrega un nuevo consentimiento al contexto de persistencia (sin SaveChanges).</summary>
    Task AddAsync(Consent consent, CancellationToken ct = default);

    /// <summary>Persiste los cambios pendientes en el contexto.</summary>
    Task SaveChangesAsync(CancellationToken ct = default);
}
