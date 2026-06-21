using AutoMapper;
using MediatR;
using TattoStudio.Application.Contracts;
using TattoStudio.Application.UseCases.Consents.Queries.GetConsentByToken;
using TattoStudio.Domain.Entities;
using TattoStudio.Domain.Exceptions;

namespace TattoStudio.Application.UseCases.Consents.Commands.SignConsent;

/// <summary>
/// Handler de <see cref="SignConsentCommand"/>. Valida el token, obtiene o crea el consentimiento,
/// verifica que no esté ya firmado y registra la firma del cliente.
/// </summary>
public sealed class SignConsentCommandHandler : IRequestHandler<SignConsentCommand, ConsentDto>
{
    private readonly IConsentTokenService   _consentTokenService;
    private readonly IConsentRepository     _consentRepository;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IMapper                _mapper;

    /// <summary>Inyecta los servicios y repositorios requeridos.</summary>
    public SignConsentCommandHandler(
        IConsentTokenService    consentTokenService,
        IConsentRepository      consentRepository,
        IAppointmentRepository  appointmentRepository,
        IMapper                 mapper)
    {
        _consentTokenService    = consentTokenService;
        _consentRepository      = consentRepository;
        _appointmentRepository  = appointmentRepository;
        _mapper                 = mapper;
    }

    /// <summary>
    /// Procesa la firma del consentimiento. Lanza <see cref="UnauthorizedException"/> si el token
    /// es inválido y <see cref="ForbiddenException"/> si el consentimiento ya fue firmado (REG-04-02).
    /// </summary>
    public async Task<ConsentDto> Handle(SignConsentCommand request, CancellationToken cancellationToken)
    {
        var appointmentId = _consentTokenService.ValidateConsentToken(request.Token);

        var consent = await _consentRepository.GetByAppointmentIdAsync(appointmentId, cancellationToken);

        if (consent is null)
        {
            var appointment = await _appointmentRepository.FindByIdAsync(appointmentId, cancellationToken)
                ?? throw new NotFoundException($"La cita con Id '{appointmentId}' no existe.");

            consent = new Consent(appointmentId, appointment.ClientId);
            await _consentRepository.AddAsync(consent, cancellationToken);
        }

        if (consent.IsSigned)
            throw new ForbiddenException("El consentimiento ya ha sido firmado y no puede modificarse.");

        consent.Sign(request.SignatureBase64);

        await _consentRepository.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ConsentDto>(consent);
    }
}
