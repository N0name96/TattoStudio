using MediatR;
using TattoStudio.Application.UseCases.Consents.Commands.SignConsent;
using TattoStudio.Application.UseCases.Consents.Queries.GetConsentByToken;

namespace TattoStudio.Api.Controllers;

/// <summary>
/// Controlador público de consentimientos. Expone la pasarela anónima para consultar y firmar
/// el formulario de consentimiento vinculado al token JWT generado por el QR (Fase 4).
/// GET  /api/public/consents/{token}       → consulta el estado del consentimiento (anónimo).
/// POST /api/public/consents/{token}/sign  → firma el consentimiento (anónimo).
/// </summary>
public static class ConsentsController
{
    /// <summary>Registra las rutas del controlador de consentimientos en el router de la aplicación.</summary>
    public static void MapConsentsController(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/public/consents").WithTags("Consentimientos");

        group.MapGet("/{token}", async (
            string            token,
            IMediator         mediator,
            CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetConsentByTokenQuery(token), ct);
            return Results.Ok(result);
        });

        group.MapPost("/{token}/sign", async (
            string               token,
            SignConsentRequest   request,
            IMediator            mediator,
            CancellationToken    ct) =>
        {
            var result = await mediator.Send(new SignConsentCommand(token, request.SignatureBase64), ct);
            return Results.Ok(result);
        });
    }
}

/// <summary>Payload de entrada para firmar un consentimiento.</summary>
internal record SignConsentRequest(string SignatureBase64);
