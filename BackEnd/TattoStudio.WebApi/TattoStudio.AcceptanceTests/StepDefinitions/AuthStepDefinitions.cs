using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Reqnroll;
using TattoStudio.AcceptanceTests.Support;
using Xunit;

namespace TattoStudio.AcceptanceTests.StepDefinitions;

[Binding]
/// <summary>
/// Step definitions Reqnroll para los escenarios de autenticación y registro (Fase 1).
/// Realiza llamadas HTTP reales contra la API en memoria y verifica los resultados.
/// </summary>
public class AuthStepDefinitions
{
    private readonly HttpClient _client;
    private readonly ScenarioState _state;

    public AuthStepDefinitions(HttpClient client, ScenarioState state)
    {
        _client = client;
        _state = state;
    }

    // ── Background ────────────────────────────────────────────────────────────

    [Given("la API está en ejecución")]
    public void GivenLaApiEstaEnEjecucion() => Assert.NotNull(_client);

    // ── Given ─────────────────────────────────────────────────────────────────

    [Given("no existe un usuario con el email {string}")]
    public void GivenNoExisteUnUsuarioConElEmail(string email)
    {
        // Pre-condición documentada; el aislamiento de BD por escenario lo garantizará
        // la infraestructura de test (InMemory / transacción revertida) en pasos posteriores.
    }

    [Given("existe un usuario registrado con email {string} y password {string}")]
    public async Task GivenExisteUnUsuarioRegistradoConEmailYPassword(string email, string password)
    {
        var payload = new { Email = email, Password = password, Name = "Test User", Role = 1 };
        // En estado RED este POST recibirá 404 porque el endpoint aún no existe.
        await _client.PostAsJsonAsync("/api/auth/register", payload);
    }

    [Given("existe un usuario registrado con email {string} y password {string} y rol {int}")]
    public async Task GivenExisteUnUsuarioRegistradoConEmailPasswordYRol(
        string email, string password, int role)
    {
        var payload = new { Email = email, Password = password, Name = "Test Admin", Role = role };
        await _client.PostAsJsonAsync("/api/auth/register", payload);
    }

    // ── When ──────────────────────────────────────────────────────────────────

    [When("envío POST a {string} con el cuerpo:")]
    public async Task WhenEnvioPostConElCuerpo(string url, Table table)
    {
        var row = table.Rows[0];

        object payload = url.Contains("register", StringComparison.OrdinalIgnoreCase)
            ? new
            {
                Email    = row["Email"],
                Password = row["Password"],
                Name     = row.ContainsKey("Name") ? row["Name"] : "Usuario",
                Role     = row.ContainsKey("Role") ? int.Parse(row["Role"]) : 1
            }
            : new
            {
                Email    = row["Email"],
                Password = row["Password"]
            };

        _state.Response = await _client.PostAsJsonAsync(url, payload);
        _state.ResponseBody = await _state.Response.Content.ReadAsStringAsync();

        if (_state.Response.IsSuccessStatusCode && !string.IsNullOrWhiteSpace(_state.ResponseBody))
        {
            using var doc = JsonDocument.Parse(_state.ResponseBody);
            if (doc.RootElement.TryGetProperty("token", out var tokenProp))
                _state.JwtToken = tokenProp.GetString();
        }
    }

    // ── Then ──────────────────────────────────────────────────────────────────

    [Then("la respuesta tiene el código HTTP {int}")]
    public void ThenLaRespuestaTieneElCodigoHttp(int expectedStatusCode)
    {
        Assert.Equal((HttpStatusCode)expectedStatusCode, _state.Response!.StatusCode);
    }

    [Then("el cuerpo de la respuesta contiene un Id de tipo Guid válido")]
    public void ThenElCuerpoContieneUnGuidValido()
    {
        using var doc = JsonDocument.Parse(_state.ResponseBody);
        Assert.True(
            doc.RootElement.TryGetProperty("id", out var idProp),
            "El cuerpo de respuesta no contiene la propiedad 'id'");
        Assert.True(
            Guid.TryParse(idProp.GetString(), out _),
            $"El valor de 'id' no es un Guid válido: {idProp.GetString()}");
    }

    [Then("el cuerpo de la respuesta contiene un token JWT no vacío")]
    public void ThenElCuerpoContieneUnTokenJwtNoVacio()
    {
        using var doc = JsonDocument.Parse(_state.ResponseBody);
        Assert.True(
            doc.RootElement.TryGetProperty("token", out var tokenProp),
            "El cuerpo de respuesta no contiene la propiedad 'token'");
        var token = tokenProp.GetString();
        Assert.False(string.IsNullOrWhiteSpace(token), "El token JWT está vacío");
        Assert.Equal(3, token!.Split('.').Length);  // header.payload.signature
    }

    [Then("el cuerpo de error contiene el mensaje {string}")]
    public void ThenElCuerpoDeErrorContieneElMensaje(string expectedMessage)
    {
        using var doc = JsonDocument.Parse(_state.ResponseBody);

        // Compatibilidad con RFC 7807 ProblemDetails ("detail") y respuestas propias ("message")
        string? detail = null;
        if (doc.RootElement.TryGetProperty("detail", out var detailProp))
            detail = detailProp.GetString();
        else if (doc.RootElement.TryGetProperty("message", out var msgProp))
            detail = msgProp.GetString();
        else
            detail = _state.ResponseBody;

        Assert.Contains(expectedMessage, detail ?? string.Empty, StringComparison.OrdinalIgnoreCase);
    }

    [Then("el token JWT contiene el claim {string}")]
    public void ThenElTokenJwtContieneElClaim(string claimName)
    {
        Assert.NotNull(_state.JwtToken);
        using var payload = DecodeJwtPayload(_state.JwtToken!);
        Assert.True(
            payload.RootElement.TryGetProperty(claimName, out _),
            $"El payload del JWT no contiene el claim '{claimName}'");
    }

    [Then("el token JWT contiene el claim {string} con valor {string}")]
    public void ThenElTokenJwtContieneElClaimConValor(string claimName, string expectedValue)
    {
        Assert.NotNull(_state.JwtToken);
        using var payload = DecodeJwtPayload(_state.JwtToken!);
        Assert.True(
            payload.RootElement.TryGetProperty(claimName, out var valueProp),
            $"El payload del JWT no contiene el claim '{claimName}'");
        Assert.Equal(expectedValue, valueProp.GetString());
    }

    [Then("el token JWT expira exactamente en 8 horas desde su emisión")]
    public void ThenElTokenJwtExpiraEn8Horas()
    {
        Assert.NotNull(_state.JwtToken);
        using var payload = DecodeJwtPayload(_state.JwtToken!);

        Assert.True(payload.RootElement.TryGetProperty("iat", out var iatProp), "JWT sin claim 'iat'");
        Assert.True(payload.RootElement.TryGetProperty("exp", out var expProp), "JWT sin claim 'exp'");

        var iat = DateTimeOffset.FromUnixTimeSeconds(iatProp.GetInt64());
        var exp = DateTimeOffset.FromUnixTimeSeconds(expProp.GetInt64());

        Assert.Equal(TimeSpan.FromHours(8), exp - iat);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static JsonDocument DecodeJwtPayload(string token)
    {
        var parts = token.Split('.');
        Assert.Equal(3, parts.Length);
        var padded = PadBase64(parts[1]);
        var bytes  = Convert.FromBase64String(padded);
        return JsonDocument.Parse(Encoding.UTF8.GetString(bytes));
    }

    private static string PadBase64(string base64)
    {
        base64 = base64.Replace('-', '+').Replace('_', '/');
        return (base64.Length % 4) switch
        {
            2 => base64 + "==",
            3 => base64 + "=",
            _ => base64
        };
    }
}
