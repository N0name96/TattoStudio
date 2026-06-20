using System.Net.Http.Json;
using System.Text.Json;
using Reqnroll;
using TattoStudio.AcceptanceTests.Support;
using Xunit;

namespace TattoStudio.AcceptanceTests.StepDefinitions;

[Binding]
public class ArtistsStepDefinitions
{
    private readonly HttpClient _client;
    private readonly ScenarioState _state;

    public ArtistsStepDefinitions(HttpClient client, ScenarioState state)
    {
        _client = client;
        _state = state;
    }

    // ── When ──────────────────────────────────────────────────────────────────

    [When("envío POST autenticado a {string} con datos del artista:")]
    public async Task WhenEnvioPostAutenticadoConDatosDelArtista(string url, Table table)
    {
        var row = table.Rows[0];

        var payload = new
        {
            Name                 = row["Name"],
            Specialty            = row["Specialty"],
            CommissionPercentage = decimal.Parse(row["CommissionPercentage"],
                                       System.Globalization.CultureInfo.InvariantCulture)
        };

        _state.Response     = await _client.PostAsJsonAsync(url, payload);
        _state.ResponseBody = await _state.Response.Content.ReadAsStringAsync();
    }

    [When("envío GET a {string}")]
    public async Task WhenEnvioGetA(string url)
    {
        _state.Response     = await _client.GetAsync(url);
        _state.ResponseBody = await _state.Response.Content.ReadAsStringAsync();
    }

    // ── Then ──────────────────────────────────────────────────────────────────

    [Then("el cuerpo de la respuesta es una lista JSON")]
    public void ThenElCuerpoEsUnaListaJson()
    {
        Assert.False(string.IsNullOrWhiteSpace(_state.ResponseBody),
            "El cuerpo de la respuesta está vacío");

        using var doc = JsonDocument.Parse(_state.ResponseBody);
        Assert.Equal(JsonValueKind.Array, doc.RootElement.ValueKind);
    }
}
