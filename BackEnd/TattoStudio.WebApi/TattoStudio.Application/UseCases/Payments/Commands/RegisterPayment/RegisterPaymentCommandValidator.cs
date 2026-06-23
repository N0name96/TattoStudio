using FluentValidation;

namespace TattoStudio.Application.UseCases.Payments.Commands.RegisterPayment;

/// <summary>Validador FluentValidation para <see cref="RegisterPaymentCommand"/>.</summary>
public class RegisterPaymentCommandValidator : AbstractValidator<RegisterPaymentCommand>
{
    /// <summary>Define las reglas de validación del comando.</summary>
    public RegisterPaymentCommandValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("El importe debe ser positivo.");
    }
}
