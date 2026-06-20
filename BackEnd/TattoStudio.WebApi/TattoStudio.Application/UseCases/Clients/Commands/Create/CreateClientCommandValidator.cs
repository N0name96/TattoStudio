using FluentValidation;

namespace TattoStudio.Application.UseCases.Clients.Commands.Create;

/// <summary>
/// Validador FluentValidation para <see cref="CreateClientCommand"/>.
/// Verifica formato, longitud y obligatoriedad de los campos del cliente.
/// </summary>
public sealed class CreateClientCommandValidator : AbstractValidator<CreateClientCommand>
{
    public CreateClientCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(150).WithMessage("El nombre no puede superar los 150 caracteres.");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("El teléfono es obligatorio.")
            .MaximumLength(20).WithMessage("El teléfono no puede superar los 20 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es obligatorio.")
            .EmailAddress().WithMessage("El email no tiene un formato válido.")
            .MaximumLength(150).WithMessage("El email no puede superar los 150 caracteres.");

        RuleFor(x => x.BirthDate)
            .NotEmpty().WithMessage("La fecha de nacimiento es obligatoria.");
    }
}
