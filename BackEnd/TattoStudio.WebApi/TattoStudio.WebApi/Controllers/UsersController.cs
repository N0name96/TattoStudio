using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TattoStudio.Application.DTOs.Users;
using TattoStudio.Application.UsesCases.Commands.Users;
using TattoStudio.Application.UsesCases.Queries.Users;

namespace TattoStudio.WebApi.Controllers;

/// <summary>Gestión de usuarios del sistema.</summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize(Roles = "Admin")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator) => _mediator = mediator;

    /// <summary>Obtiene todos los usuarios.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllUsersQuery(), cancellationToken);
        return Ok(result);
    }

    /// <summary>Obtiene un usuario por su identificador.</summary>
    /// <param name="id">Identificador único del usuario.</param>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UserDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetUserByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    /// <summary>Actualiza el rol o estado activo de un usuario existente.</summary>
    /// <param name="id">Identificador del usuario a actualizar.</param>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(UserDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id, [FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateUserCommand(id, request), cancellationToken);
        return Ok(result);
    }

    /// <summary>Elimina un usuario por su identificador.</summary>
    /// <param name="id">Identificador del usuario a eliminar.</param>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var requestingUserIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        var requestingUserId = requestingUserIdClaim is not null
            ? Guid.Parse(requestingUserIdClaim)
            : Guid.Empty;

        await _mediator.Send(new DeleteUserCommand(id, requestingUserId), cancellationToken);
        return NoContent();
    }
}
