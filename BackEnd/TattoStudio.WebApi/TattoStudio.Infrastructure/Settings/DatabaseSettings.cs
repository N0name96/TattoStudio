namespace TattoStudio.Infrastructure.Settings;

/// <summary>
/// Mapea la sección "ConnectionStrings" del appsettings.json.
/// Registro: services.Configure&lt;DatabaseSettings&gt;(config.GetSection("ConnectionStrings"))
/// </summary>
public sealed class DatabaseSettings
{
    public const string SectionName = "ConnectionStrings";

    public string DefaultConnection { get; init; } = string.Empty;
}
