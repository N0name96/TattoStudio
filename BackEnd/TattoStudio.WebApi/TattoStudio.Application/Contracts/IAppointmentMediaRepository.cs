using TattoStudio.Domain.Entities;

namespace TattoStudio.Application.Contracts;

/// <summary>
/// Contrato de persistencia para la entidad <see cref="AppointmentMedia"/>.
/// </summary>
public interface IAppointmentMediaRepository
{
    /// <summary>Persiste un nuevo registro de multimedia en el contexto (sin SaveChanges).</summary>
    Task AddAsync(AppointmentMedia media, CancellationToken ct = default);

    /// <summary>Busca un registro de multimedia por su identificador único.</summary>
    Task<AppointmentMedia?> FindByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>Elimina el registro de multimedia del contexto (sin SaveChanges).</summary>
    Task RemoveAsync(AppointmentMedia media, CancellationToken ct = default);

    /// <summary>Persiste los cambios pendientes en el contexto.</summary>
    Task SaveChangesAsync(CancellationToken ct = default);
}
