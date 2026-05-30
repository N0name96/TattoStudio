using MediatR;
using TattoStudio.Application.DTOs.Appoinments;

namespace TattoStudio.Application.UsesCases.Commands.Appoinments
{
    public record UpdateAppoinmentCommand(Guid AppoinmentId, UpdateAppoinmentRequest Data)
        : IRequest<AppoinmentDTO>;
}
