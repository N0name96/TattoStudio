using TattoStudio.Domain.Enums;

namespace TattoStudio.Application.DTOs.Users;

public record UpdateUserRequest
{
    public UserRole? Role { get; init; }
    public bool? IsActive { get; init; }
}
