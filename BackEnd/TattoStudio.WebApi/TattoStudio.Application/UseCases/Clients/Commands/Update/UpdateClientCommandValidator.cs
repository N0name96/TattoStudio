using FluentValidation;

namespace TattoStudio.Application.UseCases.Clients.Commands.Update;

/// <summary>
/// Validador FluentValidation para <see cref="UpdateClientCommand"/>.
/// Verifica obligatoriedad y formato de los campos actualizables del cliente.
/// </summary>
public sealed class UpdateClientCommandValidator : AbstractValidator<UpdateClientCommand>
{
    /// <summary>Define las reglas de validación del comando de actualización.</summary>
    public UpdateClientCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es obligatorio.");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("El teléfono es obligatorio.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es obligatorio.")
            .EmailAddress().WithMessage("El email no tiene un formato válido.");
    }
}
