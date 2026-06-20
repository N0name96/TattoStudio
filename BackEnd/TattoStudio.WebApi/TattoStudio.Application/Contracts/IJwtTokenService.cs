using TattoStudio.Domain.Entities;

namespace TattoStudio.Application.Contracts;

/// <summary>
/// Contrato para la generación de tokens JWT (implementado en Infrastructure con HS256).
/// </summary>
public interface IJwtTokenService
{
    string GenerateToken(User user);
}
