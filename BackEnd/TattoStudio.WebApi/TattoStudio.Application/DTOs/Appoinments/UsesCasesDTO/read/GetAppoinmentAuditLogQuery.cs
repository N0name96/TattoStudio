using MediatR;

namespace TattoStudio.Application.DTOs.Appoinments;

public record GetAppoinmentAuditLogQuery(
    Guid AppoinmentId,
    Guid RequestingUserId,
    bool IsAdmin
) : IRequest<IEnumerable<AppoinmentAuditLogDTO>>;
