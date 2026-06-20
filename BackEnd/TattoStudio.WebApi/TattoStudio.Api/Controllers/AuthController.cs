using MediatR;
using TattoStudio.Application.UseCases.Auth.Commands.Login;
using TattoStudio.Application.UseCases.Auth.Commands.Register;

namespace TattoStudio.Api.Controllers;

/// <summary>
/// Controlador de autenticación. Expone los endpoints POST /api/auth/register y POST /api/auth/login.
/// </summary>
public static class AuthController
{
    public static void MapAuthController(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth");

        group.MapPost("/register", async (
            RegisterRequest   request,
            IMediator         mediator,
            CancellationToken ct) =>
        {
            var result = await mediator.Send(
                new RegisterUserCommand(request.Email, request.Password, request.Name, request.Role), ct);

            return Results.Created($"/api/users/{result.Id}", new { id = result.Id });
        });

        group.MapPost("/login", async (
            LoginRequest      request,
            IMediator         mediator,
            CancellationToken ct) =>
        {
            var result = await mediator.Send(
                new LoginUserCommand(request.Email, request.Password), ct);

            return Results.Ok(new { token = result.Token });
        });
    }
}

internal record RegisterRequest(string Email, string Password, string Name, int Role);
internal record LoginRequest(string Email, string Password);
