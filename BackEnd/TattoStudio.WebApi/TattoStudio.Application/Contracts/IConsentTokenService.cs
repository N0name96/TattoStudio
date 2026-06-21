namespace TattoStudio.Application.Contracts;

/// <summary>
/// Contrato del servicio de tokens JWT de corta duración para el flujo de consentimiento.
/// Los tokens generados son independientes de los tokens de autenticación de usuario (REG-04-01).
/// </summary>
public interface IConsentTokenService
{
    /// <summary>
    /// Genera un token JWT HS256 válido por 30 minutos que embebe el <paramref name="appointmentId"/>
    /// como claim. Este token se incluye en el QR generado para la firma de consentimiento.
    /// </summary>
    /// <param name="appointmentId">Identificador de la cita a la que se vincula el consentimiento.</param>
    /// <returns>Token JWT firmado como cadena de texto.</returns>
    string GenerateConsentToken(Guid appointmentId);

    /// <summary>
    /// Valida el token y extrae el <c>appointmentId</c> embebido.
    /// </summary>
    /// <param name="token">Token JWT a validar.</param>
    /// <returns>El <see cref="Guid"/> del appointmentId si el token es válido.</returns>
    /// <exception cref="TattoStudio.Domain.Exceptions.UnauthorizedException">
    /// Se lanza si el token es inválido, está expirado o mal formado.
    /// </exception>
    Guid ValidateConsentToken(string token);
}
