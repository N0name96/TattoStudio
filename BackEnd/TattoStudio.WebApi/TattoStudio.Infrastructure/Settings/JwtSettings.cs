namespace TattoStudio.Infrastructure.Settings;

/// <summary>
/// Mapea la sección "JwtSettings" del appsettings.json.
/// Registro: services.Configure&lt;JwtSettings&gt;(config.GetSection(JwtSettings.SectionName))
/// </summary>
public sealed class JwtSettings
{
    public const string SectionName = "JwtSettings";

    public string SecretKey      { get; init; } = string.Empty;
    public string Issuer         { get; init; } = string.Empty;
    public string Audience       { get; init; } = string.Empty;
    public int    ExpiresInHours { get; init; } = 8;
}
