using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace TattoStudio.Infrastructure.Persistence;

/// <summary>
/// Instancia el DbContext en tiempo de diseño para que 'dotnet ef' pueda generar migraciones
/// sin levantar la API completa.
///
/// Comando de uso (desde la raíz de la solución):
///   dotnet ef migrations add InitialCreate -p TattoStudio.Infrastructure -s TattoStudio.Api -o Persistence/Migrations
///   dotnet ef database update            -p TattoStudio.Infrastructure -s TattoStudio.Api
/// </summary>
public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<TattoStudioDbContext>
{
    public TattoStudioDbContext CreateDbContext(string[] args)
    {
        var basePath = ResolveAppsettingsDirectory();

        var config = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .AddJsonFile("appsettings.Development.json", optional: true,  reloadOnChange: false)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = config.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                $"La clave 'ConnectionStrings:DefaultConnection' no existe en appsettings.json " +
                $"(buscado en: {basePath})");

        var options = new DbContextOptionsBuilder<TattoStudioDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new TattoStudioDbContext(options);
    }

    /// <summary>
    /// Busca el directorio que contiene el appsettings.json del proyecto Api.
    /// Soporta ejecución desde la raíz de la solución, desde Infrastructure o desde cualquier subdirectorio.
    /// </summary>
    private static string ResolveAppsettingsDirectory()
    {
        var candidates = new[]
        {
            // Caso 1: ejecutado desde la raíz de la solución
            Path.Combine(Directory.GetCurrentDirectory(), "TattoStudio.Api"),
            // Caso 2: ejecutado desde dentro del proyecto Infrastructure
            Path.Combine(Directory.GetCurrentDirectory(), "../TattoStudio.Api"),
            // Caso 3: appsettings.json está en el directorio actual (e.g. startup project ya es Api)
            Directory.GetCurrentDirectory(),
        };

        foreach (var candidate in candidates)
        {
            var normalized = Path.GetFullPath(candidate);
            if (File.Exists(Path.Combine(normalized, "appsettings.json")))
                return normalized;
        }

        throw new FileNotFoundException(
            "No se encontró appsettings.json en ninguna ruta candidata. " +
            "Ejecuta el comando 'dotnet ef' desde la raíz de la solución.");
    }
}
