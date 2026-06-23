using System.Net.Http.Json;
using System.Text.Json;
using Reqnroll;
using TattoStudio.AcceptanceTests.Support;
using Xunit;

namespace TattoStudio.AcceptanceTests.StepDefinitions;

/// <summary>
/// Step definitions Reqnroll para los escenarios de Finanzas y Logística (Fase 5).
/// Cubre registro de pagos, cierre transaccional de citas y gestión de stock.
/// </summary>
[Binding]
public class FinanceStepDefinitions
{
    private readonly HttpClient   _client;
    private readonly ScenarioState _state;

    /// <summary>Inicializa la clase con el cliente HTTP y el estado compartido del escenario.</summary>
    public FinanceStepDefinitions(HttpClient client, ScenarioState state)
    {
        _client = client;
        _state  = state;
    }

    // ── Given ────────────────────────────────────────────────────────────────

    /// <summary>Crea un artista con el porcentaje de comisión indicado y lo almacena en ScenarioState.</summary>
    [Given("existe un artista de prueba con comisión del {string} por ciento")]
    public async Task GivenExisteUnArtistaConComision(string comisionStr)
    {
        var commission = decimal.Parse(comisionStr, System.Globalization.CultureInfo.InvariantCulture);
        var payload = new
        {
            Name                 = "Artista Finance Test",
            Specialty            = "Realismo",
            CommissionPercentage = commission
        };
        var response = await _client.PostAsJsonAsync("/api/artists", payload);
        var body     = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode && !string.IsNullOrWhiteSpace(body))
        {
            using var doc = JsonDocument.Parse(body);
            if (doc.RootElement.TryGetProperty("id", out var idProp) &&
                Guid.TryParse(idProp.GetString(), out var id))
                _state.LastArtistId = id;
        }
    }

    /// <summary>Firma el consentimiento de la última cita creada para habilitar su cierre.</summary>
    [Given("el consentimiento de la última cita ha sido firmado")]
    public async Task GivenElConsentimientoDeLaUltimaCitaHaSidoFirmado()
    {
        var id = _state.LastCreatedId ?? Guid.Empty;

        // 1. Genera el QR para obtener el token de consentimiento
        var qrResp = await _client.GetAsync($"/api/appointments/{id}/qr");
        var qrBody = await qrResp.Content.ReadAsStringAsync();
        string? token = null;

        if (qrResp.IsSuccessStatusCode && !string.IsNullOrWhiteSpace(qrBody))
        {
            using var doc = JsonDocument.Parse(qrBody);
            if (doc.RootElement.TryGetProperty("token", out var tokenProp))
                token = tokenProp.GetString();
        }

        // 2. Firma el consentimiento con la pasarela pública
        if (!string.IsNullOrWhiteSpace(token))
        {
            var payload = new { SignatureBase64 = "AAEC/w==" };
            await _client.PostAsJsonAsync($"/api/public/consents/{token}/sign", payload);
        }
    }

    /// <summary>Crea un ítem de stock con los parámetros indicados.</summary>
    [Given("existe un ítem de stock con nombre {string} cantidad {string} y umbral mínimo {string}")]
    public async Task GivenExisteUnItemDeStock(string name, string cantidad, string umbral)
    {
        var payload = new
        {
            Name            = name,
            CurrentQuantity = int.Parse(cantidad),
            MinThreshold    = int.Parse(umbral)
        };
        await _client.PostAsJsonAsync("/api/stock", payload);
    }

    /// <summary>Crea una cita pendiente SIN firmar el consentimiento (para probar el rechazo de cierre).</summary>
    [Given("existe una cita sin consentimiento firmado el {string} a las {string} UTC con duración {int} hora")]
    public async Task GivenExisteUnaCitaSinConsentimientoFirmado(string date, string time, int durationHours)
    {
        var dateTime = DateTime.Parse(
            $"{date}T{time}:00Z",
            null,
            System.Globalization.DateTimeStyles.RoundtripKind);

        var payload = new
        {
            ClientId      = _state.LastClientId,
            ArtistId      = _state.LastArtistId,
            DateTime      = dateTime,
            DurationHours = durationHours,
            DepositAmount = 0.00m
        };

        var response = await _client.PostAsJsonAsync("/api/appointments", payload);
        var body     = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode && !string.IsNullOrWhiteSpace(body))
        {
            using var doc = JsonDocument.Parse(body);
            if (doc.RootElement.TryGetProperty("id", out var idProp) &&
                Guid.TryParse(idProp.GetString(), out var id))
                _state.LastCreatedId = id;
        }
        // Deliberadamente NO se firma el consentimiento para este escenario
    }

    // ── When ─────────────────────────────────────────────────────────────────

    /// <summary>Envía POST a /api/payments usando la última cita creada con los datos de la tabla.</summary>
    [When("envío POST autenticado a {string} con los datos de pago:")]
    public async Task WhenEnvioPostAPaymentsConDatosDePago(string url, Table table)
    {
        var row     = table.Rows[0];
        var payload = new
        {
            AppointmentId = _state.LastCreatedId ?? Guid.Empty,
            Amount        = decimal.Parse(row["Amount"], System.Globalization.CultureInfo.InvariantCulture),
            Type          = int.Parse(row["Type"]),
            Method        = int.Parse(row["Method"])
        };
        _state.Response     = await _client.PostAsJsonAsync(url, payload);
        _state.ResponseBody = await _state.Response.Content.ReadAsStringAsync();
    }

    /// <summary>Envía POST a /api/payments usando un appointmentId explícito con los datos de la tabla.</summary>
    [When("envío POST autenticado a {string} en la cita {string} con los datos:")]
    public async Task WhenEnvioPostAPaymentsEnCitaEspecifica(string url, string appointmentId, Table table)
    {
        var row     = table.Rows[0];
        var payload = new
        {
            AppointmentId = Guid.Parse(appointmentId),
            Amount        = decimal.Parse(row["Amount"], System.Globalization.CultureInfo.InvariantCulture),
            Type          = int.Parse(row["Type"]),
            Method        = int.Parse(row["Method"])
        };
        _state.Response     = await _client.PostAsJsonAsync(url, payload);
        _state.ResponseBody = await _state.Response.Content.ReadAsStringAsync();
    }

    /// <summary>Envía POST a /api/appointments/{id}/complete usando la última cita creada.</summary>
    [When("envío POST autenticado para completar la última cita usando el ítem {string} con cantidad {int}")]
    public async Task WhenEnvioPostParaCompletarUltimaCita(string itemName, int quantity)
    {
        var id      = _state.LastCreatedId ?? Guid.Empty;
        var payload = new
        {
            StockItems = new[] { new { StockItemName = itemName, Quantity = quantity } }
        };
        _state.Response     = await _client.PostAsJsonAsync($"/api/appointments/{id}/complete", payload);
        _state.ResponseBody = await _state.Response.Content.ReadAsStringAsync();
    }

    /// <summary>Envía POST a la URL indicada con una tabla de ítems de stock.</summary>
    [When("envío POST autenticado a {string} con items:")]
    public async Task WhenEnvioPostAutenticadoAUrlConItems(string url, Table table)
    {
        var items = table.Rows.Select(r => new
        {
            StockItemName = r["StockItemName"],
            Quantity      = int.Parse(r["Quantity"])
        }).ToList();

        var payload = new { StockItems = items };
        _state.Response     = await _client.PostAsJsonAsync(url, payload);
        _state.ResponseBody = await _state.Response.Content.ReadAsStringAsync();
    }

    // ── Then ─────────────────────────────────────────────────────────────────

    /// <summary>Verifica que el cuerpo de la respuesta contiene la propiedad status con el valor esperado.</summary>
    [Then("el cuerpo de la respuesta contiene el estado {string}")]
    public void ThenElCuerpoContieneElEstado(string expectedStatus)
    {
        using var doc = JsonDocument.Parse(_state.ResponseBody);
        Assert.True(
            doc.RootElement.TryGetProperty("status", out var statusProp),
            "El cuerpo de respuesta no contiene la propiedad 'status'");
        Assert.Equal(expectedStatus, statusProp.GetString());
    }

    /// <summary>Verifica que requiresRestock es false en el cuerpo de la respuesta.</summary>
    [Then("el cuerpo de la respuesta contiene requiresRestock false")]
    public void ThenElCuerpoContieneRequiresRestockFalse()
    {
        using var doc = JsonDocument.Parse(_state.ResponseBody);
        Assert.True(
            doc.RootElement.TryGetProperty("requiresRestock", out var prop),
            "El cuerpo de respuesta no contiene 'requiresRestock'");
        Assert.False(prop.GetBoolean(), "requiresRestock debería ser false");
    }

    /// <summary>Verifica que requiresRestock es true en el cuerpo de la respuesta.</summary>
    [Then("el cuerpo de la respuesta contiene requiresRestock true")]
    public void ThenElCuerpoContieneRequiresRestockTrue()
    {
        using var doc = JsonDocument.Parse(_state.ResponseBody);
        Assert.True(
            doc.RootElement.TryGetProperty("requiresRestock", out var prop),
            "El cuerpo de respuesta no contiene 'requiresRestock'");
        Assert.True(prop.GetBoolean(), "requiresRestock debería ser true");
    }

    /// <summary>Verifica que la lista JSON contiene al menos un ítem con requiresRestock true.</summary>
    [Then("la lista contiene al menos un ítem con requiresRestock true")]
    public void ThenLaListaContieneAlMenosUnItemConRequiresRestockTrue()
    {
        using var doc  = JsonDocument.Parse(_state.ResponseBody);
        var array = doc.RootElement;
        Assert.Equal(JsonValueKind.Array, array.ValueKind);
        var hasAny = array.EnumerateArray()
            .Any(e => e.TryGetProperty("requiresRestock", out var p) && p.GetBoolean());
        Assert.True(hasAny, "La lista no contiene ningún ítem con requiresRestock = true");
    }

    /// <summary>Verifica que la lista JSON no contiene ningún ítem con el nombre indicado.</summary>
    [Then("la lista no contiene el ítem {string}")]
    public void ThenLaListaNoContieneElItem(string name)
    {
        using var doc  = JsonDocument.Parse(_state.ResponseBody);
        var array = doc.RootElement;
        Assert.Equal(JsonValueKind.Array, array.ValueKind);
        var found = array.EnumerateArray()
            .Any(e => e.TryGetProperty("name", out var n) &&
                      string.Equals(n.GetString(), name, StringComparison.OrdinalIgnoreCase));
        Assert.False(found, $"La lista contiene el ítem '{name}' cuando no debería");
    }
}
