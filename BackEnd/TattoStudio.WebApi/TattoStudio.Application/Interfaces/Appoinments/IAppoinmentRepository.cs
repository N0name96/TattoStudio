using TattoStudio.Application.DTOs.Appoinments;
using TattoStudio.Domain.Enums;

namespace TattoStudio.Application.Interfaces
{
    public interface IAppoinmentRepository
    {
        Task<AppoinmentDTO> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<IEnumerable<AppoinmentDTO>> GetAllAsync(CancellationToken cancellationToken);
        Task<AppoinmentDTO> CreateAsync(CreateAppoinmentCommand request, CancellationToken cancellationToken);
        Task<AppoinmentDTO> UpdateAsync(Guid appoinmentId, UpdateAppoinmentRequest request, Guid changedByUserId, CancellationToken cancellationToken);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken);
        Task<AppoinmentDTO> ChangeStatusAsync(Guid id, AppoinmentStatus newStatus, string? cancellationReason, Guid changedByUserId, CancellationToken cancellationToken);
        Task<IEnumerable<AppoinmentAuditLogDTO>> GetAuditLogAsync(Guid appoinmentId, CancellationToken cancellationToken);
        Task<IEnumerable<AppoinmentDTO>> GetByArtistAndDateAsync(Guid artistId, DateOnly date, CancellationToken cancellationToken);
    }
}
