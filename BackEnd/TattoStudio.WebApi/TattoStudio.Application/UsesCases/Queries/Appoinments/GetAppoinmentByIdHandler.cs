using MediatR;
using AutoMapper;
using TattoStudio.Application.DTOs.Appoinments;
using TattoStudio.Application.Interfaces;

namespace TattoStudio.Application.UsesCases.Queries.Appoinments
{
    public class GetAppoinmentByIdHandler : IRequestHandler<GetAppoinmentByIdQuery, AppoinmentDTO>
    {
        private readonly IAppoinmentRepository appoinmentRepository;

        public GetAppoinmentByIdHandler(IAppoinmentRepository appoinmentRepository)
        {
            this.appoinmentRepository = appoinmentRepository;
        }

        public async Task<AppoinmentDTO> Handle(GetAppoinmentByIdQuery request, CancellationToken cancellationToken)
        {
            return await appoinmentRepository.GetByIdAsync(request.Id, cancellationToken);
        }
    }
}
