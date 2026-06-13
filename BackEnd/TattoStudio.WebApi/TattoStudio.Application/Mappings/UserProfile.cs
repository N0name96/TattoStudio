using AutoMapper;
using TattoStudio.Application.DTOs.Users;
using TattoStudio.Domain.Entities;

namespace TattoStudio.Application.Mappings;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<AppUser, UserDTO>();

        CreateMap<RegisterUserCommand, AppUser>()
            .ForMember(e => e.PasswordHash, opt => opt.Ignore())
            .ForMember(e => e.Id,           opt => opt.Ignore())
            .ForMember(e => e.IsActive,     opt => opt.Ignore())
            .ForMember(e => e.CreatedAt,    opt => opt.Ignore());

        CreateMap<UpdateUserRequest, AppUser>()
            .ForMember(e => e.Id,           opt => opt.Ignore())
            .ForMember(e => e.Email,        opt => opt.Ignore())
            .ForMember(e => e.PasswordHash, opt => opt.Ignore())
            .ForMember(e => e.CreatedAt,    opt => opt.Ignore())
            .ForMember(e => e.Role,         opt => opt.Condition(src => src.Role.HasValue))
            .ForMember(e => e.IsActive,     opt => opt.Condition(src => src.IsActive.HasValue));
    }
}
