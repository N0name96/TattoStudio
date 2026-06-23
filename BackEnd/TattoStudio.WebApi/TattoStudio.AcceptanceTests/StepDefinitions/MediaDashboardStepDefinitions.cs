using System.Text.Json;
using Reqnroll;
using TattoStudio.AcceptanceTests.Support;
using Xunit;

namespace TattoStudio.AcceptanceTests.StepDefinitions;

/// <summary>
/// Definiciones de pasos Gherkin para los escenarios de Multimedia y Dashboard (Fase 7).
/// </summary>
[Binding]
public class MediaDashboardStepDefinitions
{
    private readonly HttpClient    _client;
    private readonly ScenarioState _state;

    /// <summary>Inyecta el cliente HTTP y el estado del escenario.</summary>
    public MediaDashboardStepDefinitions(HttpClient client, ScenarioState state)
    {
        _client = client;
        _state  = state;
    }

    // ── Given ────────────────────────────────────────────────────────────────

    /// <summary>Sube una imagen a la última cita con el tipo de media indicado y guarda su Id.</summary>
    [Given("existe una imagen subida a la última cita con mediaType {int}")]
    public async Task GivenExisteUnaImagenSubida(int mediaType)
    {
        var bytes    = System.Text.Encoding.UTF8.GetBytes("fake-jpeg-content");
        var content  = BuildMultipart("sketch.jpg", "image/jpeg", bytes, mediaType);
        var id       = _state.LastCreatedId ?? Guid.Empty;
        var response = await _client.PostAsync($"/api/appointments/{id}/media", content);
        var body     = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode && !string.IsNullOrWhiteSpace(body))
        {
            using var doc = JsonDocument.Parse(body);
            if (doc.RootElement.TryGetProperty("id", out var idProp) &&
                Guid.TryParse(idProp.GetString(), out var mediaId))
                _state.LastMediaId = mediaId;
        }
    }

    // ── When ─────────────────────────────────────────────────────────────────

    /// <summary>Sube una imagen al endpoint de la última cita y guarda la respuesta.</summary>
    [When("envío POST autenticado con imagen {string} tipo {string} mediaType {int} a la última cita")]
    public async Task WhenEnvioPostConImagen(string fileName, string mimeType, int mediaType)
    {
        var bytes   = System.Text.Encoding.UTF8.GetBytes("fake-image-content");
        var content = BuildMultipart(fileName, mimeType, bytes, mediaType);
        var id      = _state.LastCreatedId ?? Guid.Empty;
        _state.Response     = await _client.PostAsync($"/api/appointments/{id}/media", content);
        _state.ResponseBody = await _state.Response.Content.ReadAsStringAsync();
    }

    /// <summary>Sube una imagen grande al endpoint de la última cita y guarda la respuesta.</summary>
    [When("envío POST autenticado con imagen de {int} MB tipo {string} mediaType {int} a la última cita")]
    public async Task WhenEnvioPostConImagenGrande(int sizeMb, string mimeType, int mediaType)
    {
        var bytes   = new byte[sizeMb * 1024 * 1024 + 1];
        var content = BuildMultipart("large.jpg", mimeType, bytes, mediaType);
        var id      = _state.LastCreatedId ?? Guid.Empty;
        _state.Response     = await _client.PostAsync($"/api/appointments/{id}/media", content);
        _state.ResponseBody = await _state.Response.Content.ReadAsStringAsync();
    }

    /// <summary>Envía DELETE al endpoint del último media de la última cita y guarda la respuesta.</summary>
    [When("envío DELETE autenticado al último media de la última cita")]
    public async Task WhenEnvioDeleteAlUltimoMedia()
    {
        var appointmentId = _state.LastCreatedId ?? Guid.Empty;
        var mediaId       = _state.LastMediaId   ?? Guid.Empty;
        _state.Response     = await _client.DeleteAsync($"/api/appointments/{appointmentId}/media/{mediaId}");
        _state.ResponseBody = await _state.Response.Content.ReadAsStringAsync();
    }

    /// <summary>Envía DELETE a la URL indicada y guarda la respuesta.</summary>
    [When("envío DELETE autenticado a {string}")]
    public async Task WhenEnvioDeleteA(string url)
    {
        _state.Response     = await _client.DeleteAsync(url);
        _state.ResponseBody = await _state.Response.Content.ReadAsStringAsync();
    }

    // ── Then ─────────────────────────────────────────────────────────────────

    /// <summary>Verifica que el campo totalAppointments de la respuesta es igual al valor esperado.</summary>
    [Then("el cuerpo de la respuesta contiene totalAppointments igual a {int}")]
    public void ThenTotalAppointmentsIgualA(int expected)
    {
        using var doc = JsonDocument.Parse(_state.ResponseBody);
        Assert.True(
            doc.RootElement.TryGetProperty("totalAppointments", out var prop),
            "No contiene 'totalAppointments'");
        Assert.Equal(expected, prop.GetInt32());
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    /// <summary>Construye un MultipartFormDataContent con el archivo y el tipo de media indicados.</summary>
    private static MultipartFormDataContent BuildMultipart(
        string fileName,
        string mimeType,
        byte[] bytes,
        int    mediaType)
    {
        var content     = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(bytes);
        fileContent.Headers.ContentType =
            System.Net.Http.Headers.MediaTypeHeaderValue.Parse(mimeType);
        content.Add(fileContent, "file", fileName);
        content.Add(new StringContent(mediaType.ToString()), "mediaType");
        return content;
    }
}
