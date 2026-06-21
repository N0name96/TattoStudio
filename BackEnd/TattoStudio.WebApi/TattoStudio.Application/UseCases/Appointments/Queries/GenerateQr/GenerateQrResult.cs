namespace TattoStudio.Application.UseCases.Appointments.Queries.GenerateQr;

/// <summary>
/// Resultado de la operación <see cref="GenerateQrQuery"/>.
/// Contiene la imagen QR en Base64 y el token JWT de consentimiento embebido.
/// </summary>
/// <param name="QrBase64">Imagen PNG del código QR codificada en Base64.</param>
/// <param name="Token">Token JWT de corta duración que identifica la cita para la firma del consentimiento.</param>
public record GenerateQrResult(string QrBase64, string Token);
