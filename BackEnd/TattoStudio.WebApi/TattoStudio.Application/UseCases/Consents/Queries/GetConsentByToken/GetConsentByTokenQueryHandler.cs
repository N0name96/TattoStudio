using AutoMapper;
using MediatR;
using TattoStudio.Application.Contracts;
using TattoStudio.Domain.Entities;
using TattoStudio.Domain.Exceptions;

namespace TattoStudio.Application.UseCases.Consents.Queries.GetConsentByToken;

/// <summary>
/// Handler de <see cref="GetConsentByTokenQuery"/>. Valida el token JWT de consentimiento,
/// recupera o crea el consentimiento para la cita y devuelve su estado actual.
/// </summary>
public sealed class GetConsentByTokenQueryHandler : IRequestHandler<GetConsentByTokenQuery, ConsentDto>
{
    private readonly IConsentTokenService  _consentTokenService;
    private readonly IConsentRepository   _consentRepository;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IMapper               _mapper;

    /// <summary>Inyecta los servicios y repositorios requeridos.</summary>
    public GetConsentByTokenQueryHandler(
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
    /// Valida el token y devuelve el consentimiento, creándolo si no existe aún.
    /// Lanza <see cref="UnauthorizedException"/> si el token no es válido o ha expirado.
    /// </summary>
    public async Task<ConsentDto> Handle(GetConsentByTokenQuery request, CancellationToken cancellationToken)
    {
        var appointmentId = _consentTokenService.ValidateConsentToken(request.Token);

        var consent = await _consentRepository.GetByAppointmentIdAsync(appointmentId, cancellationToken);

        if (consent is null)
        {
            var appointment = await _appointmentRepository.FindByIdAsync(appointmentId, cancellationToken)
                ?? throw new NotFoundException($"La cita con Id '{appointmentId}' no existe.");

            consent = new Consent(appointmentId, appointment.ClientId);
            await _consentRepository.AddAsync(consent, cancellationToken);
            await _consentRepository.SaveChangesAsync(cancellationToken);
        }

        return _mapper.Map<ConsentDto>(consent);
    }
}
