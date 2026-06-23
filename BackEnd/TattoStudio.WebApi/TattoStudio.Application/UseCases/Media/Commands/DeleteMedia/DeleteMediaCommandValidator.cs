using FluentValidation;

namespace TattoStudio.Application.UseCases.Media.Commands.DeleteMedia;

/// <summary>
/// Validador FluentValidation para el comando <see cref="DeleteMediaCommand"/>.
/// </summary>
public sealed class DeleteMediaCommandValidator : AbstractValidator<DeleteMediaCommand>
{
    /// <summary>Define las reglas de validación del comando.</summary>
    public DeleteMediaCommandValidator()
    {
        RuleFor(x => x.MediaId).NotEmpty();
    }
}
