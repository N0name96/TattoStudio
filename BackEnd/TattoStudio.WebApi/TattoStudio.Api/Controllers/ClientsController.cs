using MediatR;
using TattoStudio.Application.UseCases.Clients.Commands.Create;
using TattoStudio.Application.UseCases.Clients.Queries.GetById;

namespace TattoStudio.Api.Controllers;

/// <summary>
/// Controlador de clientes. Expone POST /api/clients y GET /api/clients/{id}.
/// </summary>
public static class ClientsController
{
    /// <summary>Registra las rutas del controlador de clientes en el router de la aplicación.</summary>
    public static void MapClientsController(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/clients");

        group.MapPost("/", async (
            CreateClientRequest request,
            IMediator           mediator,
            CancellationToken   ct) =>
        {
            var result = await mediator.Send(
                new CreateClientCommand(
                    request.Name,
                    request.Phone,
                    request.Email,
                    request.BirthDate,
                    request.MedicalNotes), ct);

            return Results.Created($"/api/clients/{result.Id}", new { id = result.Id });
        });

        group.MapGet("/{id:guid}", async (
            Guid              id,
            IMediator         mediator,
            CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetClientByIdQuery(id), ct);

            return Results.Ok(new
            {
                id           = result.Id,
                name         = result.Name,
                phone        = result.Phone,
                email        = result.Email,
                birthDate    = result.BirthDate,
                medicalNotes = result.MedicalNotes
            });
        });
    }
}

internal record CreateClientRequest(
    string   Name,
    string   Phone,
    string   Email,
    DateOnly BirthDate,
    string?  MedicalNotes);
