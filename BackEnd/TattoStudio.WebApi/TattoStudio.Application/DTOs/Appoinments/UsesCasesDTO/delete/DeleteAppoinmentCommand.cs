using MediatR;

namespace TattoStudio.Application.UsesCases.Commands.Appoinments
{
    public record DeleteAppoinmentCommand(Guid Id) : IRequest<Unit>;
}
