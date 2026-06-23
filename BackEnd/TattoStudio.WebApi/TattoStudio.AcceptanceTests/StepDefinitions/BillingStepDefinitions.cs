using System.Net.Http.Json;
using System.Text.Json;
using Reqnroll;
using TattoStudio.AcceptanceTests.Support;
using Xunit;

namespace TattoStudio.AcceptanceTests.StepDefinitions;

/// <summary>
/// Definiciones de pasos Gherkin para los escenarios de Facturación y VeriFactu (Fase 6).
/// </summary>
[Binding]
public class BillingStepDefinitions
{
    private readonly HttpClient    _client;
    private readonly ScenarioState _state;

    /// <summary>Inyecta el cliente HTTP y el estado del escenario.</summary>
    public BillingStepDefinitions(HttpClient client, ScenarioState state)
    {
        _client = client;
        _state  = state;
    }

    // ── Given ────────────────────────────────────────────────────────────────

    /// <summary>Crea un pago del importe indicado para la última cita y guarda su Id.</summary>
    [Given("existe un pago de {string} tipo {int} método {int} en la última cita")]
    public async Task GivenExisteUnPago(string amount, int type, int method)
    {
        var payload = new
        {
            AppointmentId = _state.LastCreatedId ?? Guid.Empty,
            Amount        = decimal.Parse(amount, System.Globalization.CultureInfo.InvariantCulture),
            Type          = type,
            Method        = method
        };
        var response = await _client.PostAsJsonAsync("/api/payments", payload);
        var body     = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode && !string.IsNullOrWhiteSpace(body))
        {
            using var doc = JsonDocument.Parse(body);
            if (doc.RootElement.TryGetProperty("id", out var idProp) &&
                Guid.TryParse(idProp.GetString(), out var id))
                _state.LastPaymentId = id;
        }
    }

    /// <summary>Emite una factura para el último pago registrado y guarda su Id.</summary>
    [Given("existe una factura emitida para el último pago")]
    public async Task GivenExisteUnaFacturaEmitida()
    {
        var payload  = new { PaymentId = _state.LastPaymentId ?? Guid.Empty };
        var response = await _client.PostAsJsonAsync("/api/billing/invoices", payload);
        var body     = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode && !string.IsNullOrWhiteSpace(body))
        {
            using var doc = JsonDocument.Parse(body);
            if (doc.RootElement.TryGetProperty("id", out var idProp) &&
                Guid.TryParse(idProp.GetString(), out var id))
                _state.LastInvoiceId = id;
        }
    }

    // ── When ─────────────────────────────────────────────────────────────────

    /// <summary>Envía POST a la URL indicada con el último pago y guarda la respuesta.</summary>
    [When("envío POST autenticado a {string} con el último pago")]
    public async Task WhenEnvioPostConElUltimoPago(string url)
    {
        var payload = new { PaymentId = _state.LastPaymentId ?? Guid.Empty };
        _state.Response     = await _client.PostAsJsonAsync(url, payload);
        _state.ResponseBody = await _state.Response.Content.ReadAsStringAsync();
        if (_state.Response.IsSuccessStatusCode && !string.IsNullOrWhiteSpace(_state.ResponseBody))
        {
            using var doc = JsonDocument.Parse(_state.ResponseBody);
            if (doc.RootElement.TryGetProperty("id", out var idProp) &&
                Guid.TryParse(idProp.GetString(), out var id))
                _state.LastInvoiceId = id;
        }
    }

    /// <summary>Envía GET al endpoint de estado VeriFactu de la última factura y guarda la respuesta.</summary>
    [When("envío GET autenticado al estado VeriFactu de la última factura")]
    public async Task WhenEnvioGetAlEstadoVeriFactu()
    {
        var id = _state.LastInvoiceId ?? Guid.Empty;
        _state.Response     = await _client.GetAsync($"/api/billing/invoices/{id}/verifactu-status");
        _state.ResponseBody = await _state.Response.Content.ReadAsStringAsync();
    }

    /// <summary>Envía POST a la URL indicada con la tabla de tokens OAuth y guarda la respuesta.</summary>
    [When("envío POST autenticado a {string} con los tokens:")]
    public async Task WhenEnvioPostConLosTokens(string url, Table table)
    {
        var row     = table.Rows[0];
        var payload = new
        {
            AccessToken  = row["AccessToken"],
            RefreshToken = row["RefreshToken"],
            ExpiresAt    = DateTimeOffset.Parse(row["ExpiresAt"])
        };
        _state.Response     = await _client.PostAsJsonAsync(url, payload);
        _state.ResponseBody = await _state.Response.Content.ReadAsStringAsync();
    }

    // ── Then ─────────────────────────────────────────────────────────────────

    /// <summary>Verifica que el campo previousInvoiceHash de la respuesta no está vacío.</summary>
    [Then("el cuerpo de la respuesta contiene previousInvoiceHash no vacío")]
    public void ThenPreviousInvoiceHashNoVacio()
    {
        using var doc = JsonDocument.Parse(_state.ResponseBody);
        Assert.True(
            doc.RootElement.TryGetProperty("previousInvoiceHash", out var hashProp),
            "No contiene 'previousInvoiceHash'");
        Assert.False(
            string.IsNullOrEmpty(hashProp.GetString()),
            "previousInvoiceHash está vacío en la segunda factura");
    }
}
