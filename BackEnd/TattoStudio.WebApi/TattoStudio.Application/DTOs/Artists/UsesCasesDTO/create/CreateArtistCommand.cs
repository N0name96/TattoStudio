using System.ComponentModel.DataAnnotations;
using MediatR;

namespace TattoStudio.Application.DTOs.Artists;

public record CreateArtistCommand : IRequest<ArtistDTO>
{
    [Required]
    [MaxLength(100)]
    public string Name { get; init; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Surname { get; init; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(150)]
    public string Mail { get; init; } = string.Empty;

    [MaxLength(20)]
    public string? PhoneNumber { get; init; }

    [Required]
    [Range(0, 100)]
    public decimal Comision { get; init; }
}
