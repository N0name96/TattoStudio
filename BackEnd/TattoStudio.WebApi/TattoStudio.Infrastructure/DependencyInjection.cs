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
        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<IArtistRepository, ArtistRepository>();
        services.AddScoped<IAppointmentRepository, AppointmentRepository>();
        services.AddScoped<IConsentRepository, ConsentRepository>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IConsentTokenService, ConsentTokenService>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IStockItemRepository, StockItemRepository>();
        services.AddScoped<IArtistCommissionsLogRepository, ArtistCommissionsLogRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IInvoiceRepository, InvoiceRepository>();
        services.AddScoped<IGoogleOAuthTokenRepository, GoogleOAuthTokenRepository>();
        services.AddScoped<IVeriFactuService, NullVeriFactuService>();
        services.AddScoped<IGoogleCalendarService, NullGoogleCalendarService>();
        services.AddScoped<IAppointmentMediaRepository, AppointmentMediaRepository>();
        services.AddScoped<ISupabaseStorageService, NullSupabaseStorageService>();
        services.AddScoped<IDashboardRepository, DashboardRepository>();

        return services;
    }
}
