namespace TattoStudio.Domain.Enums;

/// <summary>
/// Tipo de archivo multimedia asociado a una cita.
/// </summary>
public enum MediaType : short
{
    /// <summary>Imagen de referencia aportada por el cliente.</summary>
    ReferenciaCliente = 0,

    /// <summary>Boceto realizado por el artista.</summary>
    BocetoArtista = 1,

    /// <summary>Fotografía del resultado final del tatuaje.</summary>
    ResultadoFinal = 2
}
