using MediatR;
using TattoStudio.Application.UseCases.Clients.Commands.Create;
using TattoStudio.Application.UseCases.Clients.Commands.Update;
using TattoStudio.Application.UseCases.Clients.Queries.GetAll;
using TattoStudio.Application.UseCases.Clients.Queries.GetById;

namespace TattoStudio.Api.Controllers;

/// <summary>
/// Controlador de clientes. Expone POST, GET (lista), GET {id} y PUT {id} sobre /api/clients.
/// </summary>
public static class ClientsController
{
    /// <summary>Registra las rutas del controlador de clientes en el router de la aplicación.</summary>
    public static void MapClientsController(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/clients").RequireAuthorization("Staff").WithTags("Clientes");

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

        group.MapGet("/", async (
            IMediator         mediator,
            CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetAllClientsQuery(), ct);
            return Results.Ok(result);
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

        group.MapPut("/{id:guid}", async (
            Guid                id,
            UpdateClientRequest request,
            IMediator           mediator,
            CancellationToken   ct) =>
        {
            var result = await mediator.Send(
                new UpdateClientCommand(id, request.Name, request.Phone, request.Email, request.BirthDate, request.MedicalNotes), ct);

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

internal record UpdateClientRequest(
    string   Name,
    string   Phone,
    string   Email,
    DateOnly BirthDate,
    string?  MedicalNotes);
