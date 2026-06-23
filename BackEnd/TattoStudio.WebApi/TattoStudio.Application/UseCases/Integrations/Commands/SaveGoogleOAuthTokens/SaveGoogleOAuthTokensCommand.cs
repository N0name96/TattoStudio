using MediatR;

namespace TattoStudio.Application.UseCases.Integrations.Commands.SaveGoogleOAuthTokens;

/// <summary>
/// Comando para guardar o actualizar los tokens OAuth de Google Calendar de un usuario.
/// </summary>
public record SaveGoogleOAuthTokensCommand(
    Guid           UserId,
    string         AccessToken,
    string         RefreshToken,
    DateTimeOffset ExpiresAt) : IRequest;
