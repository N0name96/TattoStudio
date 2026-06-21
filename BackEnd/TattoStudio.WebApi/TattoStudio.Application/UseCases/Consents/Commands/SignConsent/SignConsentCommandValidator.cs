using FluentValidation;

namespace TattoStudio.Application.UseCases.Consents.Commands.SignConsent;

/// <summary>
/// Validador FluentValidation para <see cref="SignConsentCommand"/>.
/// Garantiza que el token y la firma estén presentes antes de que el handler ejecute la lógica de negocio.
/// </summary>
public sealed class SignConsentCommandValidator : AbstractValidator<SignConsentCommand>
{
    /// <summary>Define las reglas de validación del comando de firma de consentimiento.</summary>
    public SignConsentCommandValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty()
            .WithMessage("El token de consentimiento es obligatorio.");

        RuleFor(x => x.SignatureBase64)
            .NotEmpty()
            .WithMessage("La firma no puede estar vacía.");
    }
}
