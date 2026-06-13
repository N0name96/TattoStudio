using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TattoStudio.Application.DTOs.StudioSettings;

namespace TattoStudio.WebApi.Controllers;

/// <summary>Gestión de la configuración del estudio (horario de trabajo).</summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize(Roles = "Admin")]
public class StudioSettingsController : ControllerBase
{
    private readonly IMediator _mediator;

    public StudioSettingsController(IMediator mediator) => _mediator = mediator;

    /// <summary>Obtiene todas las configuraciones del estudio.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<StudioSettingsDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllStudioSettingsQuery(), cancellationToken);
        return Ok(result);
    }

    /// <summary>Crea una nueva configuración de estudio.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(StudioSettingsDTO), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create(
        [FromBody] CreateStudioSettingsCommand request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return CreatedAtAction(nameof(GetAll), result);
    }

    /// <summary>Actualiza la configuración del estudio por su identificador.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(StudioSettingsDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id, [FromBody] UpdateStudioSettingsRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateStudioSettingsCommand(id, request), cancellationToken);
        return Ok(result);
    }

    /// <summary>Elimina la configuración del estudio por su identificador.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteStudioSettingsCommand(id), cancellationToken);
        return NoContent();
    }
}
