using System.Net.Http.Json;
using System.Text.Json;
using Reqnroll;
using TattoStudio.AcceptanceTests.Support;
using Xunit;

namespace TattoStudio.AcceptanceTests.StepDefinitions;

/// <summary>
/// Step definitions Reqnroll para los escenarios del motor de citas (Fase 3).
/// Realiza llamadas HTTP reales contra la API en memoria y verifica los resultados.
/// </summary>
[Binding]
public class AppointmentsStepDefinitions
{
    private readonly HttpClient _client;
    private readonly ScenarioState _state;

    private Guid _artistId;
    private Guid _clientId;

    /// <summary>Inicializa la clase con el cliente HTTP y el estado compartido del escenario.</summary>
    public AppointmentsStepDefinitions(HttpClient client, ScenarioState state)
    {
        _client = client;
        _state = state;
    }

    // ── Given ─────────────────────────────────────────────────────────────────

    /// <summary>Crea un artista genérico y almacena su Id para usarlo en las citas del escenario.</summary>
    [Given("existe un artista de prueba para citas")]
    public async Task GivenExisteUnArtistaDePrueba()
    {
        var payload = new { Name = "Artista Test", Specialty = "Realismo", CommissionPercentage = 40.00m };
        var response = await _client.PostAsJsonAsync("/api/artists", payload);
        var body = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode && !string.IsNullOrWhiteSpace(body))
        {
            using var doc = JsonDocument.Parse(body);
            if (doc.RootElement.TryGetProperty("id", out var idProp) &&
                Guid.TryParse(idProp.GetString(), out var id))
                _artistId = id;
        }
    }

    /// <summary>Crea un cliente genérico y almacena su Id para usarlo en las citas del escenario.</summary>
    [Given("existe un cliente de prueba para citas")]
    public async Task GivenExisteUnClienteDePrueba()
    {
        var payload = new
        {
            Name      = "Cliente Test",
            Phone     = "600000001",
            Email     = "clientetest@tattostudio.com",
            BirthDate = "1990-05-15"
        };
        var response = await _client.PostAsJsonAsync("/api/clients", payload);
        var body = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode && !string.IsNullOrWhiteSpace(body))
        {
            using var doc = JsonDocument.Parse(body);
            if (doc.RootElement.TryGetProperty("id", out var idProp) &&
                Guid.TryParse(idProp.GetString(), out var id))
                _clientId = id;
        }
    }

    /// <summary>Registra una cita previa para el artista de prueba y provoca el escenario de solapamiento.</summary>
    [Given("el artista de prueba tiene una cita el {string} a las {string} UTC con duración {int} horas")]
    public async Task GivenElArtistaYaTieneUnaCita(string date, string time, int durationHours)
    {
        var dateTime = DateTime.Parse(
            $"{date}T{time}:00Z",
            null,
            System.Globalization.DateTimeStyles.RoundtripKind);

        var payload = new
        {
            ClientId      = _clientId,
            ArtistId      = _artistId,
            DateTime      = dateTime,
            DurationHours = durationHours,
            DepositAmount = 0.00m
        };

        var response = await _client.PostAsJsonAsync("/api/appointments", payload);
        var body = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode && !string.IsNullOrWhiteSpace(body))
        {
            using var doc = JsonDocument.Parse(body);
            if (doc.RootElement.TryGetProperty("id", out var idProp) &&
                Guid.TryParse(idProp.GetString(), out var id))
                _state.LastCreatedId = id;
        }
    }

    /// <summary>Crea una cita pendiente (sin seña) para que sea confirmada en el paso When.</summary>
    [Given("existe una cita pendiente el {string} a las {string} UTC con duración {int} horas")]
    public async Task GivenExisteUnaCitaPendiente(string date, string time, int durationHours)
    {
        var dateTime = DateTime.Parse(
            $"{date}T{time}:00Z",
            null,
            System.Globalization.DateTimeStyles.RoundtripKind);

        var payload = new
        {
            ClientId      = _clientId,
            ArtistId      = _artistId,
            DateTime      = dateTime,
            DurationHours = durationHours,
            DepositAmount = 0.00m
        };

        var response = await _client.PostAsJsonAsync("/api/appointments", payload);
        var body = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode && !string.IsNullOrWhiteSpace(body))
        {
            using var doc = JsonDocument.Parse(body);
            if (doc.RootElement.TryGetProperty("id", out var idProp) &&
                Guid.TryParse(idProp.GetString(), out var id))
                _state.LastCreatedId = id;
        }
    }

    // ── When ──────────────────────────────────────────────────────────────────

    /// <summary>Envía POST a /api/appointments con los datos del escenario y almacena la respuesta.</summary>
    [When("envío POST autenticado a {string} con datos de la cita:")]
    public async Task WhenEnvioPostAutenticadoConDatosDeLaCita(string url, Table table)
    {
        var row = table.Rows[0];

        var payload = new
        {
            ClientId      = _clientId,
            ArtistId      = _artistId,
            DateTime      = DateTime.Parse(
                row["DateTime"],
                null,
                System.Globalization.DateTimeStyles.RoundtripKind),
            DurationHours = int.Parse(row["DurationHours"]),
            DepositAmount = decimal.Parse(
                row["DepositAmount"],
                System.Globalization.CultureInfo.InvariantCulture)
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

    /// <summary>Envía PUT a /api/appointments/{id}/confirm-deposit con el importe indicado.</summary>
    [When("envío PUT autenticado para confirmar depósito de {string} en la última cita creada")]
    public async Task WhenEnvioPutConfirmarDeposito(string depositAmountStr)
    {
        var depositAmount = decimal.Parse(
            depositAmountStr,
            System.Globalization.CultureInfo.InvariantCulture);

        var id = _state.LastCreatedId ?? Guid.Empty;
        var payload = new { DepositAmount = depositAmount };

        _state.Response     = await _client.PutAsJsonAsync($"/api/appointments/{id}/confirm-deposit", payload);
        _state.ResponseBody = await _state.Response.Content.ReadAsStringAsync();
    }

    /// <summary>Envía GET a /api/appointments/{id} usando el Id de la última cita creada.</summary>
    [When("envío GET autenticado al endpoint de la última cita creada")]
    public async Task WhenEnvioGetAutenticadoAlEndpointDeLaUltimaCitaCreada()
    {
        var id = _state.LastCreatedId ?? Guid.Empty;
        _state.Response     = await _client.GetAsync($"/api/appointments/{id}");
        _state.ResponseBody = await _state.Response.Content.ReadAsStringAsync();
    }

    /// <summary>Envía GET al calendario filtrando por el ClientId del cliente de prueba.</summary>
    [When("envío GET autenticado al calendario filtrado por el último cliente creado")]
    public async Task WhenEnvioGetCalendarioFiltradoPorCliente()
    {
        var url = $"/api/appointments/calendar?from=2099-01-01&to=2099-12-31&clientId={_clientId}";
        _state.Response     = await _client.GetAsync(url);
        _state.ResponseBody = await _state.Response.Content.ReadAsStringAsync();
    }

    /// <summary>Envía GET al calendario filtrando por el ArtistId del artista de prueba.</summary>
    [When("envío GET autenticado al calendario filtrado por el último artista creado")]
    public async Task WhenEnvioGetCalendarioFiltradoPorArtista()
    {
        var url = $"/api/appointments/calendar?from=2099-01-01&to=2099-12-31&artistId={_artistId}";
        _state.Response     = await _client.GetAsync(url);
        _state.ResponseBody = await _state.Response.Content.ReadAsStringAsync();
    }

    // ── Then ──────────────────────────────────────────────────────────────────

    /// <summary>Verifica que el cuerpo de la respuesta incluya hasDeposit: true.</summary>
    [Then("el cuerpo de la respuesta contiene HasDeposit true")]
    public void ThenElCuerpoContieneHasDepositTrue()
    {
        using var doc = JsonDocument.Parse(_state.ResponseBody);
        Assert.True(
            doc.RootElement.TryGetProperty("hasDeposit", out var hasDepositProp),
            "El cuerpo de respuesta no contiene la propiedad 'hasDeposit'");
        Assert.True(hasDepositProp.GetBoolean(), "hasDeposit debería ser true cuando hay seña");
    }
}
