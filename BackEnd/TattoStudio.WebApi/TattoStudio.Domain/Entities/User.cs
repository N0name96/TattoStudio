using TattoStudio.Domain.Enums;

namespace TattoStudio.Domain.Entities;

/// <summary>
/// Entidad de dominio que representa a un miembro del staff del estudio.
/// </summary>
public sealed class User
{
    // Constructor para EF Core
    private User() { }

    public User(string email, string passwordHash, string name, Role role)
    {
        Id           = Guid.NewGuid();
        Email        = email;
        PasswordHash = passwordHash;
        Name         = name;
        Role         = role;
    }

    public Guid   Id           { get; private set; }
    public string Email        { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public string Name         { get; private set; } = null!;
    public Role   Role         { get; private set; }
}
