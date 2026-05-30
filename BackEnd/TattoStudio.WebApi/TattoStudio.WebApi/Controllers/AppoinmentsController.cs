using MediatR;
using Microsoft.AspNetCore.Mvc;
using TattoStudio.Application.DTOs.Appoinments;
using TattoStudio.Application.UsesCases.Commands.Appoinments;
using TattoStudio.Application.UsesCases.Queries.Appoinments;

namespace TattoStudio.WebApi.Controllers;

/// <summary>Gestión de citas del estudio de tatuajes.</summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AppoinmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AppoinmentsController(IMediator mediator) => _mediator = mediator;

    /// <summary>Obtiene una cita por su identificador.</summary>
    /// <param name="id">Identificador único de la cita.</param>
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
    /// <remarks>El campo <c>ArtistId</c> debe corresponder a un artista existente.</remarks>
    [HttpPost]
    [ProducesResponseType(typeof(AppoinmentDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        [FromBody] CreateAppoinmentCommand request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>Actualiza los campos de una cita existente. Solo se modifican los campos enviados.</summary>
    /// <param name="id">Identificador de la cita a actualizar.</param>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(AppoinmentDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id, [FromBody] UpdateAppoinmentRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateAppoinmentCommand(id, request), cancellationToken);
        return Ok(result);
    }

    /// <summary>Elimina una cita por su identificador.</summary>
    /// <param name="id">Identificador de la cita a eliminar.</param>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteAppoinmentCommand(id), cancellationToken);
        return NoContent();
    }
}
