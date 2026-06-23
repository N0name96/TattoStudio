using MediatR;

namespace TattoStudio.Application.UseCases.Media.Commands.DeleteMedia;

/// <summary>
/// Comando para eliminar un archivo multimedia de una cita.
/// </summary>
public record DeleteMediaCommand(Guid MediaId) : IRequest;
