using Microsoft.EntityFrameworkCore;
using TattoStudio.Application.Contracts;
using TattoStudio.Domain.Entities;
using TattoStudio.Infrastructure.Persistence;

namespace TattoStudio.Infrastructure.Repositories;

/// <summary>
/// Implementación EF Core del repositorio de usuarios.
/// </summary>
public sealed class UserRepository : IUserRepository
{
    private readonly TattoStudioDbContext _context;

    public UserRepository(TattoStudioDbContext context) => _context = context;

    public Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default) =>
        _context.Users.AnyAsync(u => u.Email == email, ct);

    public async Task AddAsync(User user, CancellationToken ct = default) =>
        await _context.Users.AddAsync(user, ct);

    public Task<User?> FindByEmailAsync(string email, CancellationToken ct = default) =>
        _context.Users.FirstOrDefaultAsync(u => u.Email == email, ct);

    public Task SaveChangesAsync(CancellationToken ct = default) =>
        _context.SaveChangesAsync(ct);
}
