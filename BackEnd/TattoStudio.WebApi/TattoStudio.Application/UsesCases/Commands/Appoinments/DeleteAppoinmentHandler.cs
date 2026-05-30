using MediatR;
using TattoStudio.Application.Interfaces;

namespace TattoStudio.Application.UsesCases.Commands.Appoinments
{
    public class DeleteAppoinmentHandler : IRequestHandler<DeleteAppoinmentCommand, Unit>
    {
        private readonly IAppoinmentRepository appoinmentRepository;

        public DeleteAppoinmentHandler(IAppoinmentRepository appoinmentRepository)
        {
            this.appoinmentRepository = appoinmentRepository;
        }

        public async Task<Unit> Handle(DeleteAppoinmentCommand request, CancellationToken cancellationToken)
        {
            await appoinmentRepository.DeleteAsync(request.Id, cancellationToken);
            return Unit.Value;
        }
    }
}