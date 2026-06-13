using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TattoStudio.Application.DTOs.Appoinments;
using TattoStudio.Application.DTOs.Artists;
using TattoStudio.Application.UsesCases.Commands.Artists;
using TattoStudio.Application.UsesCases.Queries.Artists;

namespace TattoStudio.WebApi.Controllers;

/// <summary>Gestión de artistas del estudio de tatuajes.</summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize(Roles = "Admin")]
public class ArtistsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ArtistsController(IMediator mediator) => _mediator = mediator;

    /// <summary>Obtiene todos los artistas. Por defecto solo devuelve artistas activos.</summary>
    /// <param name="includeInactive">Si es true, incluye también artistas inactivos.</param>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ArtistDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] bool includeInactive = false,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new GetAllArtistsQuery { IncludeInactive = includeInactive }, cancellationToken);
        return Ok(result);
    }

    /// <summary>Obtiene un artista por su identificador.</summary>
    /// <param name="id">Identificador único del artista.</param>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ArtistDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetArtistByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    /// <summary>Registra un nuevo artista.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ArtistDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        [FromBody] CreateArtistCommand request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>Actualiza los campos de un artista existente. Solo se modifican los campos enviados.</summary>
    /// <param name="id">Identificador del artista a actualizar.</param>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ArtistDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(
        Guid id, [FromBody] UpdateArtistRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateArtistCommand(id, request), cancellationToken);
        return Ok(result);
    }

    /// <summary>Elimina un artista por su identificador.</summary>
    /// <param name="id">Identificador del artista a eliminar.</param>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteArtistCommand(id), cancellationToken);
        return NoContent();
    }

    /// <summary>Devuelve los huecos disponibles de un artista en una fecha concreta.</summary>
    /// <param name="id">Identificador del artista.</param>
    /// <param name="date">Fecha en formato yyyy-MM-dd.</param>
    [HttpGet("{id:guid}/availability")]
    [ProducesResponseType(typeof(AvailabilityDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAvailability(
        Guid id, [FromQuery] DateOnly date, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetArtistAvailabilityQuery(id, date), cancellationToken);
        return Ok(result);
    }
}
