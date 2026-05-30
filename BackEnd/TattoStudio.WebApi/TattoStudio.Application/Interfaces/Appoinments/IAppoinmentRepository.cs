using TattoStudio.Application.DTOs.Appoinments;


namespace TattoStudio.Application.Interfaces
{
    public interface IAppoinmentRepository
    {
        Task<AppoinmentDTO> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<IEnumerable<AppoinmentDTO>> GetAllAsync(CancellationToken cancellationToken);
        Task<AppoinmentDTO> CreateAsync(CreateAppoinmentCommand request, CancellationToken cancellationToken);
        Task<AppoinmentDTO> UpdateAsync(Guid AppoinmentID, UpdateAppoinmentRequest request, CancellationToken cancellationToken);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken);
    }
}
