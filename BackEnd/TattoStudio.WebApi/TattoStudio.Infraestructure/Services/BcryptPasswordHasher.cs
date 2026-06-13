using TattoStudio.Application.Interfaces;

namespace TattoStudio.Infraestructure.Services;

public class BcryptPasswordHasher : IPasswordHasher
{
    public string Hash(string plainPassword) =>
        BCrypt.Net.BCrypt.HashPassword(plainPassword);

    public bool Verify(string plainPassword, string hash) =>
        BCrypt.Net.BCrypt.Verify(plainPassword, hash);
}
