using AutoMapper;
using TattoStudio.Application.UseCases.Consents.Queries.GetConsentByToken;
using TattoStudio.Domain.Entities;

namespace TattoStudio.Application.Mappings;

/// <summary>
/// Perfil AutoMapper para la entidad <see cref="Consent"/>.
/// Centraliza el mapeo hacia <see cref="ConsentDto"/> utilizado en los casos de uso de la Fase 4.
/// </summary>
public sealed class ConsentProfile : Profile
{
    /// <summary>Registra el mapeo de <see cref="Consent"/> hacia <see cref="ConsentDto"/>.</summary>
    public ConsentProfile()
    {
        CreateMap<Consent, ConsentDto>();
    }
}
