namespace TattoStudio.Domain.Exceptions;

/// <summary>
/// Excepción de dominio que el GlobalExceptionHandler mapea a HTTP 404 Not Found.
/// </summary>
public sealed class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}
