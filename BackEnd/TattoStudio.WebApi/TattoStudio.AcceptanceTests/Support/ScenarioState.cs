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
}
