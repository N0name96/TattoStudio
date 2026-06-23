using AutoMapper;
using TattoStudio.Application.UseCases.Media.Commands.UploadMedia;
using TattoStudio.Domain.Entities;

namespace TattoStudio.Application.Mappings;

/// <summary>
/// Perfil AutoMapper para la entidad <see cref="AppointmentMedia"/>.
/// Centraliza los mappings hacia resultados de comandos de multimedia.
/// </summary>
public sealed class MediaProfile : Profile
{
    /// <summary>Registra todos los mapeos de la entidad AppointmentMedia.</summary>
    public MediaProfile()
    {
        CreateMap<AppointmentMedia, UploadMediaResult>()
            .ForCtorParam("MediaType", opt => opt.MapFrom(s => (int)s.MediaType));
    }
}
