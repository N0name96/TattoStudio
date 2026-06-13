using AutoMapper;
using TattoStudio.Application.DTOs.Appoinments;
using TattoStudio.Domain.Entities;

namespace TattoStudio.Application.Mappings;

public class AppoinmentAuditLogProfile : Profile
{
    public AppoinmentAuditLogProfile()
    {
        CreateMap<AppoinmentAuditLog, AppoinmentAuditLogDTO>();
    }
}
