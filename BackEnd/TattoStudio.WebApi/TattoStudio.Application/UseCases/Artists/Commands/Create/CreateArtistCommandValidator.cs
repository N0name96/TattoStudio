using FluentValidation;

namespace TattoStudio.Application.UseCases.Artists.Commands.Create;

/// <summary>
/// Validador FluentValidation para <see cref="CreateArtistCommand"/>.
/// Aplica REG-02-02: la comisión debe estar entre 0.00 y 100.00.
/// </summary>
public sealed class CreateArtistCommandValidator : AbstractValidator<CreateArtistCommand>
{
    public CreateArtistCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(150).WithMessage("El nombre no puede superar los 150 caracteres.");

        RuleFor(x => x.Specialty)
            .NotEmpty().WithMessage("La especialidad es obligatoria.")
            .MaximumLength(100).WithMessage("La especialidad no puede superar los 100 caracteres.");

        RuleFor(x => x.CommissionPercentage)
            .InclusiveBetween(0m, 100m)
            .WithMessage("La comisión debe estar entre 0.00 y 100.00.");
    }
}
