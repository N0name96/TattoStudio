using TattoStudio.Application.Contracts;

namespace TattoStudio.Infrastructure.Services;

/// <summary>
/// Implementación de <see cref="IPasswordHasher"/> usando BCrypt.Net-Next (spec REG-01-02).
/// </summary>
public sealed class PasswordHasher : IPasswordHasher
{
    public string Hash(string password) =>
        BCrypt.Net.BCrypt.HashPassword(password);

    public bool Verify(string password, string hash) =>
        BCrypt.Net.BCrypt.Verify(password, hash);
}
