namespace TattoStudio.Domain.Exceptions;

/// <summary>
/// Excepción de dominio lanzada cuando el artista ya tiene una cita activa
/// que se solapa con el rango solicitado (REG-03-02).
/// GlobalExceptionHandler la mapea a HTTP 400 vía la cadena de herencia BusinessException.
/// </summary>
public sealed class ArtistOverbookedException : BusinessException
{
    /// <summary>Inicializa la excepción con el mensaje de solapamiento estándar.</summary>
    public ArtistOverbookedException()
        : base("El artista tiene un solapamiento de agenda en el horario solicitado.") { }
}
