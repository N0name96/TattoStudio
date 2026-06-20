using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Reqnroll;
using TattoStudio.AcceptanceTests.Support;
using Xunit;

namespace TattoStudio.AcceptanceTests.StepDefinitions;

[Binding]
public class ClientsStepDefinitions
{
    private readonly HttpClient _client;
    private readonly ScenarioState _state;

    public ClientsStepDefinitions(HttpClient client, ScenarioState state)
    {
        _client = client;
        _state = state;
    }

    // ── Given ─────────────────────────────────────────────────────────────────

    [Given("estoy autenticado con email {string} y password {string}")]
    public async Task GivenEstoyAutenticadoConEmailYPassword(string email, string password)
    {
        var payload = new { Email = email, Password = password };
        var response = await _client.PostAsJsonAsync("/api/auth/login", payload);
        var body = await response.Content.ReadAsStringAsync();

        if (!string.IsNullOrWhiteSpace(body))
        {
            using var doc = JsonDocument.Parse(body);
            if (doc.RootElement.TryGetProperty("token", out var tokenProp))
            {
                _state.JwtToken = tokenProp.GetString();
                _client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _state.JwtToken);
            }
        }
    }

    [Given("existe un cliente con email {string} y fecha de nacimiento {string}")]
    public async Task GivenExisteUnClienteConEmailYFechaDeNacimiento(string email, string birthDate)
    {
        var payload = new
        {
            Name      = "Cliente Test",
            Phone     = "600000000",
            Email     = email,
            BirthDate = birthDate
        };

        var response = await _client.PostAsJsonAsync("/api/clients", payload);
        var body     = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode && !string.IsNullOrWhiteSpace(body))
        {
            using var doc = JsonDocument.Parse(body);
            if (doc.RootElement.TryGetProperty("id", out var idProp) &&
                Guid.TryParse(idProp.GetString(), out var id))
                _state.LastCreatedId = id;
        }
    }

    // ── When ──────────────────────────────────────────────────────────────────

    [When("envío POST autenticado a {string} con datos del cliente:")]
    public async Task WhenEnvioPostAutenticadoConDatosDelCliente(string url, Table table)
    {
        var row = table.Rows[0];

        var payload = new
        {
            Name          = row["Name"],
            Phone         = row["Phone"],
            Email         = row["Email"],
            BirthDate     = row["BirthDate"],
            MedicalNotes  = row.ContainsKey("MedicalNotes") ? row["MedicalNotes"] : null
        };

        _state.Response     = await _client.PostAsJsonAsync(url, payload);
        _state.ResponseBody = await _state.Response.Content.ReadAsStringAsync();

        if (_state.Response.IsSuccessStatusCode && !string.IsNullOrWhiteSpace(_state.ResponseBody))
        {
            using var doc = JsonDocument.Parse(_state.ResponseBody);
            if (doc.RootElement.TryGetProperty("id", out var idProp) &&
                Guid.TryParse(idProp.GetString(), out var id))
                _state.LastCreatedId = id;
        }
    }

    [When("envío GET autenticado al endpoint del último cliente creado")]
    public async Task WhenEnvioGetAutenticadoAlEndpointDelUltimoClienteCreado()
    {
        var id = _state.LastCreatedId ?? Guid.Empty;
        _state.Response     = await _client.GetAsync($"/api/clients/{id}");
        _state.ResponseBody = await _state.Response.Content.ReadAsStringAsync();
    }

    [When("envío GET autenticado a {string}")]
    public async Task WhenEnvioGetAutenticadoA(string url)
    {
        _state.Response     = await _client.GetAsync(url);
        _state.ResponseBody = await _state.Response.Content.ReadAsStringAsync();
    }

    // ── Then ──────────────────────────────────────────────────────────────────

    [Then("el cuerpo de la respuesta contiene el email {string}")]
    public void ThenElCuerpoContieneElEmail(string expectedEmail)
    {
        using var doc = JsonDocument.Parse(_state.ResponseBody);
        Assert.True(
            doc.RootElement.TryGetProperty("email", out var emailProp),
            "El cuerpo de respuesta no contiene la propiedad 'email'");
        Assert.Equal(expectedEmail, emailProp.GetString(), StringComparer.OrdinalIgnoreCase);
    }
}
