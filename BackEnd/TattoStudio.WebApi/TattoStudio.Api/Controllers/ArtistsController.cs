using MediatR;
using TattoStudio.Application.UseCases.Artists.Commands.Create;
using TattoStudio.Application.UseCases.Artists.Queries.GetAll;

namespace TattoStudio.Api.Controllers;

/// <summary>
/// Controlador de artistas. Expone POST /api/artists y GET /api/artists.
/// </summary>
public static class ArtistsController
{
    /// <summary>Registra las rutas del controlador de artistas en el router de la aplicación.</summary>
    public static void MapArtistsController(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/artists");

        group.MapPost("/", async (
            CreateArtistRequest request,
            IMediator           mediator,
            CancellationToken   ct) =>
        {
            var result = await mediator.Send(
                new CreateArtistCommand(
                    request.Name,
                    request.Specialty,
                    request.CommissionPercentage), ct);

            return Results.Created($"/api/artists/{result.Id}", new { id = result.Id });
        });

        group.MapGet("/", async (
            IMediator         mediator,
            CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetAllArtistsQuery(), ct);
            return Results.Ok(result);
        });
    }
}

internal record CreateArtistRequest(
    string  Name,
    string  Specialty,
    decimal CommissionPercentage);
