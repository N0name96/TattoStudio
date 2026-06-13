using TattoStudio.Domain.Enums;

namespace TattoStudio.Application.DTOs.Users;

public record UserDTO
{
    public Guid Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public UserRole Role { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
}
