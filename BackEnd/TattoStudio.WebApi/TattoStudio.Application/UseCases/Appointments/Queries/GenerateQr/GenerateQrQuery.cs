using MediatR;

namespace TattoStudio.Application.UseCases.Appointments.Queries.GenerateQr;

/// <summary>
/// Query para generar el código QR de consentimiento de una cita existente.
/// El QR embebe un token JWT de 30 minutos que identifica la cita (REG-04-01).
/// </summary>
/// <param name="AppointmentId">Identificador de la cita para la que se genera el QR.</param>
public record GenerateQrQuery(Guid AppointmentId) : IRequest<GenerateQrResult>;
