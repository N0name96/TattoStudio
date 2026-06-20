namespace TattoStudio.Domain.Exceptions;

/// <summary>
/// Excepción de dominio que el GlobalExceptionHandler mapea a HTTP 400 (spec §6).
/// </summary>
public class BusinessException : Exception
{
    public BusinessException(string message) : base(message) { }
}
