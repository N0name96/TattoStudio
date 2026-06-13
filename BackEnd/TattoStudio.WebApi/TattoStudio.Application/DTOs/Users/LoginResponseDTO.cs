namespace TattoStudio.Application.DTOs.Users;

public record LoginResponseDTO
{
    public string Token { get; init; } = string.Empty;
    public DateTime ExpiresAt { get; init; }
}
