using Microsoft.EntityFrameworkCore;
using TattoStudio.Application.Contracts;
using TattoStudio.Domain.Entities;
using TattoStudio.Infrastructure.Persistence;

namespace TattoStudio.Infrastructure.Repositories;

/// <summary>
/// Implementación EF Core del repositorio de tokens OAuth de Google.
/// </summary>
public sealed class GoogleOAuthTokenRepository : BaseRepository<GoogleOAuthToken>, IGoogleOAuthTokenRepository
{
    /// <summary>Inyecta el DbContext principal.</summary>
    public GoogleOAuthTokenRepository(TattoStudioDbContext context) : base(context) { }

    /// <inheritdoc/>
    public Task<GoogleOAuthToken?> FindByUserIdAsync(Guid userId, CancellationToken ct = default) =>
        _context.GoogleOAuthTokens.FirstOrDefaultAsync(t => t.UserId == userId, ct);
}
