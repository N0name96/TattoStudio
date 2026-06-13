using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TattoStudio.Application.DTOs.Appoinments;
using TattoStudio.Application.UsesCases.Commands.Appoinments;
using TattoStudio.Application.UsesCases.Queries.Appoinments;

namespace TattoStudio.WebApi.Controllers;

/// <summary>Gestión de citas del estudio de tatuajes.</summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class AppoinmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AppoinmentsController(IMediator mediator) => _mediator = mediator;

    /// <summary>Obtiene una cita por su identificador.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AppoinmentDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAppoinmentByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    /// <summary>Obtiene todas las citas.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AppoinmentDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllAppoinmentsQuery(), cancellationToken);
        return Ok(result);
    }

    /// <summary>Crea una nueva cita.</summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(AppoinmentDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        [FromBody] CreateAppoinmentCommand request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>Actualiza los campos de una cita existente. Solo se modifican los campos enviados.</summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(AppoinmentDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id, [FromBody] UpdateAppoinmentRequest request, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var result = await _mediator.Send(new UpdateAppoinmentCommand(id, request, userId), cancellationToken);
        return Ok(result);
    }

    /// <summary>Cambia el estado de una cita. Permitido para Admin o el artista asignado.</summary>
    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(typeof(AppoinmentDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ChangeStatus(
        Guid id, [FromBody] ChangeAppoinmentStatusRequest request, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var isAdmin = User.IsInRole("Admin");
        var command = new ChangeAppoinmentStatusCommand(id, request.Status, request.CancellationReason, userId, isAdmin);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>Devuelve el historial de cambios de una cita.</summary>
    [HttpGet("{id:guid}/audit")]
    [ProducesResponseType(typeof(IEnumerable<AppoinmentAuditLogDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAuditLog(Guid id, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var isAdmin = User.IsInRole("Admin");
        var result = await _mediator.Send(new GetAppoinmentAuditLogQuery(id, userId, isAdmin), cancellationToken);
        return Ok(result);
    }

    /// <summary>Elimina una cita por su identificador.</summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteAppoinmentCommand(id), cancellationToken);
        return NoContent();
    }

    private Guid GetUserId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return claim is not null && Guid.TryParse(claim, out var id)
            ? id
            : throw new UnauthorizedAccessException("User identity could not be determined.");
    }
}
