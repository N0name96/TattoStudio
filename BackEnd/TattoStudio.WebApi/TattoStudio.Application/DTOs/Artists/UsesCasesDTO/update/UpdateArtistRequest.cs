using System.ComponentModel.DataAnnotations;

namespace TattoStudio.Application.DTOs.Artists;

public record UpdateArtistRequest
{
    [MaxLength(100)]
    public string? Name { get; init; }

    [MaxLength(100)]
    public string? Surname { get; init; }

    [EmailAddress]
    [MaxLength(150)]
    public string? Mail { get; init; }

    [MaxLength(20)]
    public string? PhoneNumber { get; init; }

    [Range(0, 100)]
    public decimal? Comision { get; init; }
}
