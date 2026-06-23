using TattoStudio.Domain.Entities;

namespace TattoStudio.Application.Contracts;

/// <summary>
/// Contrato de persistencia para la entidad <see cref="ArtistCommissionsLog"/>.
/// </summary>
public interface IArtistCommissionsLogRepository
{
    /// <summary>Persiste un nuevo registro de comisión en el contexto (sin SaveChanges).</summary>
    Task AddAsync(ArtistCommissionsLog log, CancellationToken ct = default);

    /// <summary>Persiste los cambios pendientes en el contexto.</summary>
    Task SaveChangesAsync(CancellationToken ct = default);
}
