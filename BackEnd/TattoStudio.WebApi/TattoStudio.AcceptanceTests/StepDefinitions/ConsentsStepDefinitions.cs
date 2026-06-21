using System.Net.Http.Json;
using System.Text.Json;
using Reqnroll;
using TattoStudio.AcceptanceTests.Support;
using Xunit;

namespace TattoStudio.AcceptanceTests.StepDefinitions;

/// <summary>
/// Step definitions Reqnroll para los escenarios del flujo legal de consentimientos y QR (Fase 4).
/// Cubre generación de QR, validación de token JWT de 30 minutos y firma inmutable del consentimiento.
/// </summary>
[Binding]
public class ConsentsStepDefinitions
{
    private readonly HttpClient _client;
    private readonly ScenarioState _state;
    private string? _consentToken;

    /// <summary>Inicializa la clase con el cliente HTTP y el estado compartido del escenario.</summary>
    public ConsentsStepDefinitions(HttpClient client, ScenarioState state)
    {
        _client = client;
        _state  = state;
    }

    // ── Given ─────────────────────────────────────────────────────────────────

    /// <summary>Genera el QR de la última cita creada y extrae el token JWT embebido para los pasos posteriores.</summary>
    [Given("el QR de la última cita ha sido generado y el token fue extraído")]
    public async Task GivenElQrHaSidoGeneradoYElTokenFueExtraido()
    {
        var id       = _state.LastCreatedId ?? Guid.Empty;
        var response = await _client.GetAsync($"/api/appointments/{id}/qr");
        var body     = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode && !string.IsNullOrWhiteSpace(body))
        {
            using var doc = JsonDocument.Parse(body);
            if (doc.RootElement.TryGetProperty("token", out var tokenProp))
                _consentToken = tokenProp.GetString();
        }
    }

    /// <summary>Firma el consentimiento mediante la pasarela pública para preparar el escenario de re-firma.</summary>
    [Given("el consentimiento ya ha sido firmado con la firma {string}")]
    public async Task GivenElConsentimientoYaHaSidoFirmado(string signatureBase64)
    {
        var payload = new { SignatureBase64 = signatureBase64 };
        await _client.PostAsJsonAsync($"/api/public/consents/{_consentToken}/sign", payload);
    }

    // ── When ──────────────────────────────────────────────────────────────────

    /// <summary>Envía GET autenticado a /api/appointments/{id}/qr y almacena la respuesta y el token.</summary>
    [When("envío GET autenticado al QR de la última cita creada")]
    public async Task WhenEnvioGetAutenticadoAlQrDeLaUltimaCitaCreada()
    {
        var id = _state.LastCreatedId ?? Guid.Empty;
        _state.Response     = await _client.GetAsync($"/api/appointments/{id}/qr");
        _state.ResponseBody = await _state.Response.Content.ReadAsStringAsync();

        if (_state.Response.IsSuccessStatusCode && !string.IsNullOrWhiteSpace(_state.ResponseBody))
        {
            using var doc = JsonDocument.Parse(_state.ResponseBody);
            if (doc.RootElement.TryGetProperty("token", out var tokenProp))
                _consentToken = tokenProp.GetString();
        }
    }

    /// <summary>Envía GET anónimo a /api/public/consents/{token} usando el token extraído del QR.</summary>
    [When("envío GET anónimo al formulario público del consentimiento")]
    public async Task WhenEnvioGetAnonimoAlFormularioPublicoDelConsentimiento()
    {
        _state.Response     = await _client.GetAsync($"/api/public/consents/{_consentToken}");
        _state.ResponseBody = await _state.Response.Content.ReadAsStringAsync();
    }

    /// <summary>Envía GET anónimo a la URL indicada directamente (p.ej. para probar tokens inválidos).</summary>
    [When("envío GET anónimo a {string}")]
    public async Task WhenEnvioGetAnonimoA(string url)
    {
        _state.Response     = await _client.GetAsync(url);
        _state.ResponseBody = await _state.Response.Content.ReadAsStringAsync();
    }

    /// <summary>Envía POST anónimo a /api/public/consents/{token}/sign usando el token extraído del QR.</summary>
    [When("envío POST anónimo para firmar el consentimiento con la firma:")]
    public async Task WhenEnvioPostAnonimoParaFirmarElConsentimiento(Table table)
    {
        var payload = new { SignatureBase64 = table.Rows[0]["SignatureBase64"] };

        _state.Response     = await _client.PostAsJsonAsync($"/api/public/consents/{_consentToken}/sign", payload);
        _state.ResponseBody = await _state.Response.Content.ReadAsStringAsync();
    }

    /// <summary>Envía POST anónimo a /api/public/consents/{tokenLiteral}/sign con un token fijo del escenario.</summary>
    [When("envío POST anónimo para firmar el consentimiento en token {string} con la firma:")]
    public async Task WhenEnvioPostAnonimoParaFirmarConTokenLiteral(string tokenLiteral, Table table)
    {
        var payload = new { SignatureBase64 = table.Rows[0]["SignatureBase64"] };

        _state.Response     = await _client.PostAsJsonAsync($"/api/public/consents/{tokenLiteral}/sign", payload);
        _state.ResponseBody = await _state.Response.Content.ReadAsStringAsync();
    }

    // ── Then ──────────────────────────────────────────────────────────────────

    /// <summary>Verifica que la propiedad qrBase64 existe y no está vacía en el cuerpo de respuesta.</summary>
    [Then("el cuerpo de la respuesta contiene una imagen QR en Base64")]
    public void ThenElCuerpoContieneUnaImagenQrEnBase64()
    {
        using var doc = JsonDocument.Parse(_state.ResponseBody);
        Assert.True(
            doc.RootElement.TryGetProperty("qrBase64", out var qrProp),
            "El cuerpo de respuesta no contiene la propiedad 'qrBase64'");
        Assert.False(string.IsNullOrWhiteSpace(qrProp.GetString()), "La imagen QR en Base64 está vacía");
    }

    /// <summary>Verifica que la propiedad token existe y contiene un JWT de tres segmentos.</summary>
    [Then("el cuerpo de la respuesta contiene un token de consentimiento no vacío")]
    public void ThenElCuerpoContieneUnTokenDeConsentimientoNoVacio()
    {
        using var doc = JsonDocument.Parse(_state.ResponseBody);
        Assert.True(
            doc.RootElement.TryGetProperty("token", out var tokenProp),
            "El cuerpo de respuesta no contiene la propiedad 'token'");
        var token = tokenProp.GetString();
        Assert.False(string.IsNullOrWhiteSpace(token), "El token de consentimiento está vacío");
        Assert.Equal(3, token!.Split('.').Length);
    }

    /// <summary>Verifica que la propiedad indicada (camelCase) existe en el cuerpo de la respuesta.</summary>
    [Then("el cuerpo de la respuesta contiene el campo {string}")]
    public void ThenElCuerpoContieneElCampo(string fieldName)
    {
        using var doc    = JsonDocument.Parse(_state.ResponseBody);
        var camelField   = char.ToLowerInvariant(fieldName[0]) + fieldName[1..];
        Assert.True(
            doc.RootElement.TryGetProperty(camelField, out _),
            $"El cuerpo de respuesta no contiene la propiedad '{camelField}'");
    }

    /// <summary>Verifica que isSigned es true en la respuesta del consentimiento firmado.</summary>
    [Then("el cuerpo de la respuesta contiene isSigned true")]
    public void ThenElCuerpoContieneIsSignedTrue()
    {
        using var doc = JsonDocument.Parse(_state.ResponseBody);
        Assert.True(
            doc.RootElement.TryGetProperty("isSigned", out var isSignedProp),
            "El cuerpo de respuesta no contiene la propiedad 'isSigned'");
        Assert.True(isSignedProp.GetBoolean(), "isSigned debería ser true tras la firma");
    }

    /// <summary>Verifica que signedAt no es nulo en la respuesta del consentimiento firmado (REG-04-03).</summary>
    [Then("el cuerpo de la respuesta contiene signedAt no nulo")]
    public void ThenElCuerpoContieneSignedAtNoNulo()
    {
        using var doc = JsonDocument.Parse(_state.ResponseBody);
        Assert.True(
            doc.RootElement.TryGetProperty("signedAt", out var signedAtProp),
            "El cuerpo de respuesta no contiene la propiedad 'signedAt'");
        Assert.NotEqual(JsonValueKind.Null, signedAtProp.ValueKind);
        Assert.False(
            string.IsNullOrWhiteSpace(signedAtProp.GetString()),
            "signedAt no debe estar vacío tras la firma");
    }
}
