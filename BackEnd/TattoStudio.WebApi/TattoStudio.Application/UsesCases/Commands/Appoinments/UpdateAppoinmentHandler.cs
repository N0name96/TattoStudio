using MediatR;
using TattoStudio.Application.DTOs.Appoinments;
using TattoStudio.Application.Interfaces;

namespace TattoStudio.Application.UsesCases.Commands.Appoinments
{
    public class UpdateAppoinmentHandler : IRequestHandler<UpdateAppoinmentCommand, AppoinmentDTO>
    {
        private readonly IAppoinmentRepository appoinmentRepository;

        public UpdateAppoinmentHandler(IAppoinmentRepository appoinmentRepository)
        {
            this.appoinmentRepository = appoinmentRepository;
        }

        public async Task<AppoinmentDTO> Handle(UpdateAppoinmentCommand request, CancellationToken cancellationToken)
        {
            return await appoinmentRepository.UpdateAsync(request.AppoinmentId, request.Data, request.ChangedByUserId, cancellationToken);
        }
    }
}