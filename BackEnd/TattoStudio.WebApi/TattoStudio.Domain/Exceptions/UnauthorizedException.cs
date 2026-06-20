namespace TattoStudio.Domain.Exceptions;

/// <summary>
/// Excepción de dominio que el GlobalExceptionHandler mapea a HTTP 401 (spec REG-01-02).
/// </summary>
public sealed class UnauthorizedException : Exception
{
    public UnauthorizedException(string message) : base(message) { }
}
