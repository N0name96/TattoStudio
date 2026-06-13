using TattoStudio.Domain.Enums;

namespace TattoStudio.Application.Interfaces;

public interface IJwtService
{
    (string Token, DateTime ExpiresAt) GenerateToken(Guid userId, string email, UserRole role);
}
