using MediatR;

namespace TattoStudio.Application.UseCases.Artists.Queries.GetAll;

/// <summary>
/// Query CQRS para obtener la lista completa de artistas del estudio.
/// </summary>
public record GetAllArtistsQuery : IRequest<IReadOnlyList<ArtistDto>>;

/// <summary>DTO de presentación de un artista en la lista pública.</summary>
public record ArtistDto(
    Guid    Id,
    string  Name,
    string  Specialty,
    decimal CommissionPercentage);
