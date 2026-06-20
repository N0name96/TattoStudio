using TattoStudio.Domain.Entities;

namespace TattoStudio.Application.Contracts;

/// <summary>
/// Contrato de persistencia para la entidad <see cref="TattoStudio.Domain.Entities.User"/>.
/// </summary>
public interface IUserRepository
{
    Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default);
    Task AddAsync(User user, CancellationToken ct = default);
    Task<User?> FindByEmailAsync(string email, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
