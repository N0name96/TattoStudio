using FluentValidation;

namespace TattoStudio.Application.UseCases.Media.Commands.UploadMedia;

/// <summary>
/// Validador FluentValidation para el comando <see cref="UploadMediaCommand"/>.
/// Las validaciones de ContentType y tamaño se realizan en el Handler para poder lanzar
/// BusinessException con mensajes específicos.
/// </summary>
public sealed class UploadMediaCommandValidator : AbstractValidator<UploadMediaCommand>
{
    /// <summary>Define las reglas de validación básicas del comando.</summary>
    public UploadMediaCommandValidator()
    {
        RuleFor(x => x.AppointmentId).NotEmpty();
        RuleFor(x => x.ContentType).NotEmpty();
    }
}
