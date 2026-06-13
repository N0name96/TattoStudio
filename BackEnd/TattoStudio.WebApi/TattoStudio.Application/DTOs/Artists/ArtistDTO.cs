namespace TattoStudio.Application.DTOs.Artists;

public record ArtistDTO
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Surname { get; init; } = string.Empty;
    public string Mail { get; init; } = string.Empty;
    public string? PhoneNumber { get; init; }
    public decimal Comision { get; init; }
    public bool IsActive { get; init; }
    public DateTime? DeactivatedAt { get; init; }
}
