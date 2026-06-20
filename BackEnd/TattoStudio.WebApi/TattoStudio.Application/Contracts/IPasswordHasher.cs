namespace TattoStudio.Application.Contracts;

/// <summary>
/// Contrato para hashear y verificar contraseñas (implementado con BCrypt en Infrastructure).
/// </summary>
public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}
