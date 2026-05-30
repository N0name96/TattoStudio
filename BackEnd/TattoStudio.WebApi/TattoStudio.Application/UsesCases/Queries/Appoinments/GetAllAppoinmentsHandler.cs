using MediatR;
using TattoStudio.Application.DTOs.Appoinments;
using TattoStudio.Application.Interfaces;

namespace TattoStudio.Application.UsesCases.Queries.Appoinments
{
    public class GetAllAppoinmentsHandler : IRequestHandler<GetAllAppoinmentsQuery, IEnumerable<AppoinmentDTO>>
    {
        private readonly IAppoinmentRepository appoinmentRepository;

        public GetAllAppoinmentsHandler(IAppoinmentRepository appoinmentRepository)
        {
            this.appoinmentRepository = appoinmentRepository;
        }

        public async Task<IEnumerable<AppoinmentDTO>> Handle(GetAllAppoinmentsQuery request, CancellationToken cancellationToken)
        {
            return await appoinmentRepository.GetAllAsync(cancellationToken);
        }
    }
}
