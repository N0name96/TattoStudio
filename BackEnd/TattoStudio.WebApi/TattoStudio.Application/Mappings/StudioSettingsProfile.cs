using AutoMapper;
using TattoStudio.Application.DTOs.StudioSettings;
using TattoStudio.Domain.Entities;

namespace TattoStudio.Application.Mappings;

public class StudioSettingsProfile : Profile
{
    public StudioSettingsProfile()
    {
        CreateMap<StudioSettings, StudioSettingsDTO>();
        CreateMap<CreateStudioSettingsCommand, StudioSettings>()
            .ForMember(e => e.Id, opt => opt.Ignore());
        CreateMap<UpdateStudioSettingsRequest, StudioSettings>()
            .ForMember(e => e.Id, opt => opt.Ignore());
    }
}
