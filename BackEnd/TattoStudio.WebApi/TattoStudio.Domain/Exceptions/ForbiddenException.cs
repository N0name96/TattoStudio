namespace TattoStudio.Domain.Exceptions;

/// <summary>
/// Excepción de dominio que el GlobalExceptionHandler mapea a HTTP 403 Forbidden.
/// Se lanza cuando se intenta una operación sobre un recurso al que el cliente no tiene
/// acceso, como re-firmar un consentimiento ya firmado (REG-04-02).
/// </summary>
public sealed class ForbiddenException : Exception
{
    /// <summary>Crea una nueva <see cref="ForbiddenException"/> con el mensaje de detalle indicado.</summary>
    /// <param name="message">Mensaje descriptivo del motivo del rechazo.</param>
    public ForbiddenException(string message) : base(message) { }
}
