using MediatR;
using TattoStudio.Application.Contracts;
using TattoStudio.Domain.Entities;

namespace TattoStudio.Application.UseCases.Integrations.Commands.SaveGoogleOAuthTokens;

/// <summary>
/// Handler que guarda o actualiza los tokens OAuth de Google Calendar de un usuario.
/// </summary>
public sealed class SaveGoogleOAuthTokensCommandHandler
    : IRequestHandler<SaveGoogleOAuthTokensCommand>
{
    private readonly IGoogleOAuthTokenRepository _tokens;

    /// <summary>Inyecta el repositorio de tokens OAuth.</summary>
    public SaveGoogleOAuthTokensCommandHandler(IGoogleOAuthTokenRepository tokens)
    {
        _tokens = tokens;
    }

    /// <summary>
    /// Busca el token existente para el usuario; si existe lo actualiza, si no crea uno nuevo.
    /// </summary>
    public async Task Handle(SaveGoogleOAuthTokensCommand cmd, CancellationToken ct)
    {
        var existing = await _tokens.FindByUserIdAsync(cmd.UserId, ct);

        if (existing is not null)
        {
            existing.Update(cmd.AccessToken, cmd.RefreshToken, cmd.ExpiresAt);
        }
        else
        {
            var token = new GoogleOAuthToken(cmd.UserId, cmd.AccessToken, cmd.RefreshToken, cmd.ExpiresAt);
            await _tokens.AddAsync(token, ct);
        }

        await _tokens.SaveChangesAsync(ct);
    }
}
