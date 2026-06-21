using Microsoft.EntityFrameworkCore;
using TattoStudio.Application.Contracts;
using TattoStudio.Domain.Entities;
using TattoStudio.Infrastructure.Persistence;

namespace TattoStudio.Infrastructure.Repositories;

/// <summary>
/// Implementación EF Core del repositorio de usuarios.
/// </summary>
public sealed class UserRepository : BaseRepository<User>, IUserRepository
{
    /// <summary>Inyecta el DbContext principal.</summary>
    public UserRepository(TattoStudioDbContext context) : base(context) { }

    /// <inheritdoc/>
    public Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default) =>
        _context.Users.AnyAsync(u => u.Email == email, ct);

    /// <inheritdoc/>
    public Task<User?> FindByEmailAsync(string email, CancellationToken ct = default) =>
        _context.Users.FirstOrDefaultAsync(u => u.Email == email, ct);
}
