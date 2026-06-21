namespace TattoStudio.Application.UseCases.Consents.Queries.GetConsentByToken;

/// <summary>
/// DTO de salida que representa el estado actual de un consentimiento.
/// Usado tanto en la consulta del formulario como en la respuesta de la firma.
/// </summary>
/// <param name="Id">Identificador único del consentimiento.</param>
/// <param name="AppointmentId">Identificador de la cita asociada.</param>
/// <param name="ClientId">Identificador del cliente que debe firmar.</param>
/// <param name="IsSigned">Indica si el consentimiento ha sido firmado.</param>
/// <param name="SignedAt">Marca de tiempo UTC de la firma. Nulo si aún no está firmado.</param>
public record ConsentDto(
    Guid             Id,
    Guid             AppointmentId,
    Guid             ClientId,
    bool             IsSigned,
    DateTimeOffset?  SignedAt);
