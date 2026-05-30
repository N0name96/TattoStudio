using MediatR;
using TattoStudio.Application.DTOs.Appoinments;

namespace TattoStudio.Application.UsesCases.Queries.Appoinments
{
    public record GetAllAppoinmentsQuery : IRequest<IEnumerable<AppoinmentDTO>>;
}
