using MediatR;
using TattoStudio.Application.DTOs.Appoinments;
using TattoStudio.Application.Interfaces;

namespace TattoStudio.Application.UsesCases.Commands.Appoinments
{
    public class CreateAppoinmentHandler : IRequestHandler<CreateAppoinmentCommand, AppoinmentDTO>
    {
        private readonly IAppoinmentRepository appoinmentRepository;

        public CreateAppoinmentHandler(IAppoinmentRepository appoinmentRepository)
        {
            this.appoinmentRepository = appoinmentRepository;
        }

        public async Task<AppoinmentDTO> Handle(CreateAppoinmentCommand request, CancellationToken cancellationToken)
        {
            return await appoinmentRepository.CreateAsync(request, cancellationToken);
        }
    }
}