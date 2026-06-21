using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace TattoStudio.Infrastructure.Persistence;

/// <summary>
/// Inicializador de base de datos. Reescribe cada sentencia DDL con IF NOT EXISTS
/// antes de ejecutarla, para crear solo las tablas e índices que aún no existen en Supabase/PostgreSQL.
/// </summary>
public static class DatabaseInitializer
{
    private static readonly Regex StatementSplitter =
        new(@";\s*\r?\n", RegexOptions.Compiled);

    /// <summary>
    /// Genera el script DDL del modelo, inyecta IF NOT EXISTS en cada sentencia
    /// y las ejecuta individualmente. Sin excepciones ni ruido de log para relaciones ya existentes.
    /// </summary>
    public static async Task InitializeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TattoStudioDbContext>();

        if (!db.Database.IsRelational()) return;

        var creator = db.Database.GetService<IRelationalDatabaseCreator>() as RelationalDatabaseCreator;
        var script  = creator!.GenerateCreateScript();

        foreach (var statement in StatementSplitter.Split(script)
                                                   .Select(s => s.Trim())
                                                   .Where(s => s.Length > 0)
                                                   .Select(ToIdempotentStatement))
        {
            await db.Database.ExecuteSqlRawAsync(statement);
        }
    }

    /// <summary>
    /// Convierte CREATE TABLE y CREATE [UNIQUE] INDEX a sus variantes IF NOT EXISTS.
    /// </summary>
    private static string ToIdempotentStatement(string sql) =>
        sql.Replace("CREATE TABLE ",        "CREATE TABLE IF NOT EXISTS ",        StringComparison.OrdinalIgnoreCase)
           .Replace("CREATE UNIQUE INDEX ", "CREATE UNIQUE INDEX IF NOT EXISTS ", StringComparison.OrdinalIgnoreCase)
           .Replace("CREATE INDEX ",        "CREATE INDEX IF NOT EXISTS ",        StringComparison.OrdinalIgnoreCase);
}
