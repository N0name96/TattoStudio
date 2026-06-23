using TattoStudio.Domain.Entities;

namespace TattoStudio.Application.Contracts;

/// <summary>
/// Contrato de persistencia para la entidad <see cref="GoogleOAuthToken"/>.
/// </summary>
public interface IGoogleOAuthTokenRepository
{
    /// <summary>Persiste un nuevo token OAuth en el contexto (sin SaveChanges).</summary>
    Task AddAsync(GoogleOAuthToken token, CancellationToken ct = default);

    /// <summary>Busca el token OAuth de Google asociado al usuario indicado.</summary>
    Task<GoogleOAuthToken?> FindByUserIdAsync(Guid userId, CancellationToken ct = default);

    /// <summary>Persiste los cambios pendientes en el contexto.</summary>
    Task SaveChangesAsync(CancellationToken ct = default);
}
