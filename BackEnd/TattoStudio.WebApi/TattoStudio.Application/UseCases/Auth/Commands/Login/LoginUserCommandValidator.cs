using FluentValidation;

namespace TattoStudio.Application.UseCases.Auth.Commands.Login;

/// <summary>
/// Validador FluentValidation para <see cref="LoginUserCommand"/>.
/// Verifica que email y contraseña no lleguen vacíos antes de consultar la base de datos.
/// </summary>
public sealed class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es obligatorio.")
            .EmailAddress().WithMessage("El email no tiene un formato válido.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es obligatoria.");
    }
}
