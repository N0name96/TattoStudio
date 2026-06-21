using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TattoStudio.Application.Contracts;
using TattoStudio.Domain.Exceptions;
using TattoStudio.Infrastructure.Settings;

namespace TattoStudio.Infrastructure.Services;

/// <summary>
/// Implementación de <see cref="IConsentTokenService"/> que genera y valida tokens JWT HS256
/// de corta duración (30 minutos) para el flujo de consentimiento (REG-04-01).
/// Reutiliza la misma clave secreta que <see cref="JwtTokenService"/> pero con expiración diferente.
/// </summary>
public sealed class ConsentTokenService : IConsentTokenService
{
    private const string AppointmentIdClaim = "appointmentId";

    private readonly JwtSettings _settings;

    /// <summary>Inyecta la configuración JWT compartida.</summary>
    public ConsentTokenService(IOptions<JwtSettings> options) =>
        _settings = options.Value;

    /// <inheritdoc/>
    public string GenerateConsentToken(Guid appointmentId)
    {
        var issuedAt  = DateTimeOffset.UtcNow;
        var expiresAt = issuedAt.AddMinutes(30);

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(AppointmentIdClaim, appointmentId.ToString()),
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

    /// <inheritdoc/>
    public Guid ValidateConsentToken(string token)
    {
        try
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey));

            var parameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey         = key,
                ValidateIssuer           = true,
                ValidIssuer              = _settings.Issuer,
                ValidateAudience         = true,
                ValidAudience            = _settings.Audience,
                ValidateLifetime         = true,
                ClockSkew                = TimeSpan.Zero
            };

            var handler    = new JwtSecurityTokenHandler();
            var principal  = handler.ValidateToken(token, parameters, out _);
            var claimValue = principal.FindFirst(AppointmentIdClaim)?.Value;

            if (claimValue is null || !Guid.TryParse(claimValue, out var appointmentId))
                throw new UnauthorizedException("Token de consentimiento inválido o expirado.");

            return appointmentId;
        }
        catch (UnauthorizedException)
        {
            throw;
        }
        catch
        {
            throw new UnauthorizedException("Token de consentimiento inválido o expirado.");
        }
    }
}
