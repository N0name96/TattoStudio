using AutoMapper;
using TattoStudio.Application.DTOs.Appoinments;
using TattoStudio.Domain.Entities;

namespace TattoStudio.Application.Mappings
{
    public class AppoinmentProfile : Profile
    {
        public AppoinmentProfile()
        {
            CreateMap<Appoinment, AppoinmentDTO>();

            CreateMap<CreateAppoinmentCommand, Appoinment>();
            CreateMap<UpdateAppoinmentRequest, Appoinment>()
                .ForMember(e => e.Id,               opt => opt.Ignore())
                .ForMember(e => e.ArtistId,         opt => opt.Condition(src => src.ArtistId.HasValue))
                .ForMember(e => e.Name,              opt => opt.Condition(src => src.Name != null))
                .ForMember(e => e.MailClient,        opt => opt.Condition(src => src.MailClient != null))
                .ForMember(e => e.PhoneNumber,       opt => opt.Condition(src => src.PhoneNumber != null))
                .ForMember(e => e.TattoImage,        opt => opt.Condition(src => src.TattoImage != null))
                .ForMember(e => e.Deposit,           opt => opt.Condition(src => src.Deposit.HasValue))
                .ForMember(e => e.DepositAmount,     opt => opt.Condition(src => src.DepositAmount.HasValue))
                .ForMember(e => e.TotalPrice,        opt => opt.Condition(src => src.TotalPrice.HasValue))
                .ForMember(e => e.AppoinmentDate,    opt => opt.Condition(src => src.AppoinmentDate.HasValue))
                .ForMember(e => e.SignedConsentForm, opt => opt.Condition(src => src.SignedConsentForm.HasValue))
                .ForMember(e => e.CreatedAt,         opt => opt.Condition(src => src.CreatedAt.HasValue))
                .ForMember(e => e.UpdatedAt,         opt => opt.Condition(src => src.UpdatedAt.HasValue));
        }
    }
}
