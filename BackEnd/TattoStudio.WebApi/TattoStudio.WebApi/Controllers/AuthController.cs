using MediatR;
using Microsoft.AspNetCore.Mvc;
using TattoStudio.Application.DTOs.Users;
using TattoStudio.Application.UsesCases.Commands.Users;

namespace TattoStudio.WebApi.Controllers;

/// <summary>Autenticación y registro de usuarios.</summary>
[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator) => _mediator = mediator;

    /// <summary>Registra un nuevo usuario.</summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(UserDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return CreatedAtAction(nameof(UsersController.GetById), "Users", new { id = result.Id }, result);
    }

    /// <summary>Autentica al usuario y devuelve un JWT.</summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        [FromBody] LoginUserCommand request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
}
