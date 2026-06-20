using FluentValidation;

namespace TattoStudio.Application.UseCases.Auth.Commands.Register;

/// <summary>
/// Validador FluentValidation para <see cref="RegisterUserCommand"/>.
/// Verifica formato y longitud antes de que el handler consulte la base de datos.
/// </summary>
public sealed class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es obligatorio.")
            .EmailAddress().WithMessage("El email no tiene un formato válido.")
            .MaximumLength(150).WithMessage("El email no puede superar los 150 caracteres.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es obligatoria.")
            .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre no puede superar los 100 caracteres.");

        RuleFor(x => x.Role)
            .InclusiveBetween(0, 2).WithMessage("El rol debe ser 0 (Admin), 1 (Recepcion) o 2 (Artista).");
    }
}
