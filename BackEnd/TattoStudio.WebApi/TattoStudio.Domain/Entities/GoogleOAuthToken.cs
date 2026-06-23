namespace TattoStudio.Domain.Entities;

/// <summary>
/// Entidad que almacena los tokens OAuth de Google Calendar de un usuario.
/// </summary>
public sealed class GoogleOAuthToken
{
    private GoogleOAuthToken() { }

    /// <summary>Crea un nuevo registro de token OAuth para el usuario indicado.</summary>
    public GoogleOAuthToken(Guid userId, string accessToken, string refreshToken, DateTimeOffset expiresAt)
    {
        Id           = Guid.NewGuid();
        UserId       = userId;
        AccessToken  = accessToken;
        RefreshToken = refreshToken;
        ExpiresAt    = expiresAt;
    }

    /// <summary>Identificador único del token.</summary>
    public Guid           Id           { get; private set; }

    /// <summary>FK al usuario propietario del token.</summary>
    public Guid           UserId       { get; private set; }

    /// <summary>Token de acceso de Google OAuth.</summary>
    public string         AccessToken  { get; private set; } = null!;

    /// <summary>Token de refresco de Google OAuth.</summary>
    public string         RefreshToken { get; private set; } = null!;

    /// <summary>Fecha y hora UTC de expiración del token de acceso.</summary>
    public DateTimeOffset ExpiresAt    { get; private set; }

    /// <summary>Actualiza los tokens OAuth cuando se renuevan.</summary>
    public void Update(string accessToken, string refreshToken, DateTimeOffset expiresAt)
    {
        AccessToken  = accessToken;
        RefreshToken = refreshToken;
        ExpiresAt    = expiresAt;
    }
}
