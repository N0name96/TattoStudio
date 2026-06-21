using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TattoStudio.Domain.Exceptions;

namespace TattoStudio.Api.Middleware;

/// <summary>
/// Mapea excepciones de dominio a respuestas RFC 7807 (ProblemDetails).
/// BusinessException → 400, UnauthorizedException → 401,
/// DbUpdateException → 409, resto → 500 (spec §6).
/// </summary>
public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetails;

    public GlobalExceptionHandler(IProblemDetailsService problemDetails) =>
        _problemDetails = problemDetails;

    public async ValueTask<bool> TryHandleAsync(
        HttpContext       context,
        Exception         exception,
        CancellationToken ct)
    {
        var (status, detail) = exception switch
        {
            BusinessException      be => (StatusCodes.Status400BadRequest,          be.Message),
            UnauthorizedException  ue => (StatusCodes.Status401Unauthorized,        ue.Message),
            NotFoundException      ne => (StatusCodes.Status404NotFound,            ne.Message),
            ForbiddenException     fe => (StatusCodes.Status403Forbidden,           fe.Message),
            DbUpdateException         => (StatusCodes.Status409Conflict,            "Conflicto de datos en base de datos."),
            _                         => (StatusCodes.Status500InternalServerError, "Error interno del servidor.")
        };

        context.Response.StatusCode = status;

        return await _problemDetails.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext    = context,
            Exception      = exception,
            ProblemDetails = new ProblemDetails
            {
                Status = status,
                Detail = detail
            }
        });
    }
}
