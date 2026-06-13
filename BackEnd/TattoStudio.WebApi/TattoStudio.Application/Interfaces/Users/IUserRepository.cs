using TattoStudio.Application.DTOs.Users;

namespace TattoStudio.Application.Interfaces;

public interface IUserRepository
{
    Task<UserDTO> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<UserDTO>> GetAllAsync(CancellationToken cancellationToken);
    Task<UserDTO> RegisterAsync(RegisterUserCommand request, CancellationToken cancellationToken);
    Task<LoginResponseDTO> LoginAsync(LoginUserCommand request, CancellationToken cancellationToken);
    Task<UserDTO> UpdateAsync(Guid userId, UpdateUserRequest request, CancellationToken cancellationToken);
    Task DeleteAsync(Guid userId, Guid requestingUserId, CancellationToken cancellationToken);
}
