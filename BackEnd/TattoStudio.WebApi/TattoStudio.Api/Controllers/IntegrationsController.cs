using System.Security.Claims;
using MediatR;
using TattoStudio.Application.UseCases.Integrations.Commands.SaveGoogleOAuthTokens;

namespace TattoStudio.Api.Controllers;

/// <summary>
/// Controlador de integraciones externas. Expone POST /api/integrations/google-auth (Fase 6).
/// </summary>
public static class IntegrationsController
{
    /// <summary>Registra las rutas del controlador de integraciones en el router de la aplicación.</summary>
    public static void MapIntegrationsController(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/integrations")
                       .RequireAuthorization("AdminOnly")
                       .WithTags("Integraciones");

        group.MapPost("/google-auth", async (
            SaveGoogleAuthRequest request,
            ClaimsPrincipal       user,
            IMediator             mediator,
            CancellationToken     ct) =>
        {
            var userId = Guid.Parse(user.FindFirst("sub")!.Value);
            await mediator.Send(
                new SaveGoogleOAuthTokensCommand(
                    userId,
                    request.AccessToken,
                    request.RefreshToken,
                    request.ExpiresAt),
                ct);
            return Results.Ok();
        });
    }
}

/// <summary>Payload de entrada para guardar tokens OAuth de Google.</summary>
internal record SaveGoogleAuthRequest(
    string         AccessToken,
    string         RefreshToken,
    DateTimeOffset ExpiresAt);
