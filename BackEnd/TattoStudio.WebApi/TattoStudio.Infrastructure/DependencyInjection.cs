using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TattoStudio.Application.Contracts;
using TattoStudio.Infrastructure.Persistence;
using TattoStudio.Infrastructure.Repositories;
using TattoStudio.Infrastructure.Services;
using TattoStudio.Infrastructure.Settings;

namespace TattoStudio.Infrastructure;

/// <summary>
/// Registro de dependencias de la capa Infrastructure: DbContext, repositorios, servicios y settings.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registra EF Core (Npgsql/Supabase), repositorios, PasswordHasher, JwtTokenService y Options.
    /// </summary>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<DatabaseSettings>(
            configuration.GetSection(DatabaseSettings.SectionName));

        services.Configure<JwtSettings>(
            configuration.GetSection(JwtSettings.SectionName));

        services.AddDbContext<TattoStudioDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "La clave 'ConnectionStrings:DefaultConnection' no está configurada.")));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        return services;
    }
}
