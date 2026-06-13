using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TattoStudio.Domain.Exceptions;

namespace TattoStudio.WebApi.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        var (status, title) = exception switch
        {
            AppoinmentNotFoundException  => (StatusCodes.Status404NotFound,            "Not Found"),
            AppoinmentConflictException  => (StatusCodes.Status409Conflict,             "Conflict"),
            ArtistNotFoundException      => (StatusCodes.Status404NotFound,            "Not Found"),
            ArtistMailConflictException  => (StatusCodes.Status409Conflict,             "Conflict"),
            UserNotFoundException        => (StatusCodes.Status404NotFound,            "Not Found"),
            UserEmailConflictException   => (StatusCodes.Status409Conflict,             "Conflict"),
            InvalidCredentialsException  => (StatusCodes.Status401Unauthorized,        "Unauthorized"),
            UserInactiveException        => (StatusCodes.Status401Unauthorized,        "Unauthorized"),
            UserSelfDeleteException      => (StatusCodes.Status409Conflict,             "Conflict"),
            _                            => (StatusCodes.Status500InternalServerError, "Internal Server Error")
        };

        var problem = new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = exception.Message,
            Instance = context.Request.Path
        };

        context.Response.StatusCode = status;
        await context.Response.WriteAsJsonAsync(problem, cancellationToken);
        return true;
    }
}
