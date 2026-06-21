using MediatR;
using QRCoder;
using TattoStudio.Application.Contracts;
using TattoStudio.Domain.Exceptions;

namespace TattoStudio.Application.UseCases.Appointments.Queries.GenerateQr;

/// <summary>
/// Handler de <see cref="GenerateQrQuery"/>. Verifica que la cita exista, genera un token JWT
/// de consentimiento de 30 minutos y lo codifica como imagen QR PNG en Base64.
/// </summary>
public sealed class GenerateQrQueryHandler : IRequestHandler<GenerateQrQuery, GenerateQrResult>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IConsentTokenService   _consentTokenService;

    /// <summary>Inyecta el repositorio de citas y el servicio de tokens de consentimiento.</summary>
    public GenerateQrQueryHandler(
        IAppointmentRepository appointmentRepository,
        IConsentTokenService   consentTokenService)
    {
        _appointmentRepository = appointmentRepository;
        _consentTokenService   = consentTokenService;
    }

    /// <summary>
    /// Genera el QR de consentimiento para la cita indicada.
    /// Lanza <see cref="NotFoundException"/> si la cita no existe.
    /// </summary>
    public async Task<GenerateQrResult> Handle(GenerateQrQuery request, CancellationToken cancellationToken)
    {
        var appointment = await _appointmentRepository.FindByIdAsync(request.AppointmentId, cancellationToken);

        if (appointment is null)
            throw new NotFoundException($"La cita con Id '{request.AppointmentId}' no existe.");

        var token = _consentTokenService.GenerateConsentToken(appointment.Id);

        var qrGenerator = new QRCodeGenerator();
        var qrData      = qrGenerator.CreateQrCode(token, QRCodeGenerator.ECCLevel.Q);
        var qrCode      = new PngByteQRCode(qrData);
        var qrBytes     = qrCode.GetGraphic(20);
        var qrBase64    = Convert.ToBase64String(qrBytes);

        return new GenerateQrResult(qrBase64, token);
    }
}
