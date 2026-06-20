using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using TattoStudio.Application.Contracts;
using TattoStudio.Domain.Entities;
using TattoStudio.Infrastructure.Settings;

namespace TattoStudio.Infrastructure.Services;

/// <summary>
/// Implementación de <see cref="IJwtTokenService"/> que genera tokens JWT HS256
/// con claims sub, email y role, e iat/exp exactos a 8 horas (spec REG-01-03).
/// </summary>
public sealed class JwtTokenService : IJwtTokenService
{
    private readonly JwtSettings _settings;

    public JwtTokenService(IOptions<JwtSettings> options) =>
        _settings = options.Value;

    public string GenerateToken(User user)
    {
        // Timestamps en segundos exactos para garantizar exp - iat = 8h exactas (spec REG-01-03)
        var issuedAt  = DateTimeOffset.UtcNow;
        var expiresAt = issuedAt.AddHours(_settings.ExpiresInHours);

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,   user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("role", ((int)user.Role).ToString()),
            }),
            IssuedAt           = issuedAt.UtcDateTime,
            Expires            = expiresAt.UtcDateTime,
            Issuer             = _settings.Issuer,
            Audience           = _settings.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey)),
                SecurityAlgorithms.HmacSha256)
        };

        return new JwtSecurityTokenHandler().CreateEncodedJwt(descriptor);
    }
}
