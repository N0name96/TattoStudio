using MediatR;
using TattoStudio.Domain.Enums;

namespace TattoStudio.Application.DTOs.Appoinments;

public record ChangeAppoinmentStatusCommand(
    Guid AppoinmentId,
    AppoinmentStatus NewStatus,
    string? CancellationReason,
    Guid RequestingUserId,
    bool IsAdmin
) : IRequest<AppoinmentDTO>;
