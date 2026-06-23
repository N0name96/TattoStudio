using FluentValidation;

namespace TattoStudio.Application.UseCases.Integrations.Commands.SaveGoogleOAuthTokens;

/// <summary>
/// Validador FluentValidation para el comando <see cref="SaveGoogleOAuthTokensCommand"/>.
/// </summary>
public sealed class SaveGoogleOAuthTokensCommandValidator : AbstractValidator<SaveGoogleOAuthTokensCommand>
{
    /// <summary>Define las reglas de validación del comando.</summary>
    public SaveGoogleOAuthTokensCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.AccessToken).NotEmpty();
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}
