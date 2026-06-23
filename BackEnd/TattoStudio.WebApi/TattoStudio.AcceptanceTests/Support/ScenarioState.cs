namespace TattoStudio.AcceptanceTests.Support;

/// <summary>
/// Bolsa de estado compartido entre pasos Gherkin de un mismo escenario.
/// Inyectada por el contenedor BoDi de Reqnroll.
/// </summary>
public class ScenarioState
{
    public HttpResponseMessage? Response { get; set; }
    public string ResponseBody { get; set; } = string.Empty;
    public string? JwtToken { get; set; }
    public Guid? LastCreatedId { get; set; }

    /// <summary>Id del último artista creado, compartido entre step definitions.</summary>
    public Guid LastArtistId { get; set; }

    /// <summary>Id del último cliente creado, compartido entre step definitions.</summary>
    public Guid LastClientId { get; set; }

    /// <summary>Id de la última factura creada, compartido entre step definitions.</summary>
    public Guid? LastInvoiceId { get; set; }

    /// <summary>Id del último archivo multimedia subido, compartido entre step definitions.</summary>
    public Guid? LastMediaId { get; set; }

    /// <summary>Id del último pago registrado, compartido entre step definitions.</summary>
    public Guid? LastPaymentId { get; set; }
}
