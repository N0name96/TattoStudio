using AutoMapper;
using TattoStudio.Application.DTOs.Artists;
using TattoStudio.Domain.Entities;

namespace TattoStudio.Application.Mappings;

public class ArtistProfile : Profile
{
    public ArtistProfile()
    {
        CreateMap<Artist, ArtistDTO>();

        CreateMap<CreateArtistCommand, Artist>();

        CreateMap<UpdateArtistRequest, Artist>()
            .ForMember(e => e.Id,          opt => opt.Ignore())
            .ForMember(e => e.Name,        opt => opt.Condition(src => src.Name != null))
            .ForMember(e => e.Surname,     opt => opt.Condition(src => src.Surname != null))
            .ForMember(e => e.Mail,        opt => opt.Condition(src => src.Mail != null))
            .ForMember(e => e.PhoneNumber, opt => opt.Condition(src => src.PhoneNumber != null))
            .ForMember(e => e.Comision,    opt => opt.Condition(src => src.Comision.HasValue));
    }
}
