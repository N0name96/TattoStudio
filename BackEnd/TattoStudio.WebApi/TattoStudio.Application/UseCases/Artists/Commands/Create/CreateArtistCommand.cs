using MediatR;

namespace TattoStudio.Application.UseCases.Artists.Commands.Create;

/// <summary>
/// Comando CQRS para registrar un nuevo artista en el sistema (Fase 2).
/// </summary>
public record CreateArtistCommand(
    string  Name,
    string  Specialty,
    decimal CommissionPercentage) : IRequest<CreateArtistResult>;

/// <summary>Resultado del comando con el identificador del artista creado.</summary>
public record CreateArtistResult(Guid Id);
