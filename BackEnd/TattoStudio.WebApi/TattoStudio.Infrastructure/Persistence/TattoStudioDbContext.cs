using Microsoft.EntityFrameworkCore;
using TattoStudio.Domain.Entities;

namespace TattoStudio.Infrastructure.Persistence;

/// <summary>
/// DbContext principal de la aplicación. Aplica todas las configuraciones Fluent API
/// del ensamblado de Infrastructure via <see cref="ModelBuilder.ApplyConfigurationsFromAssembly"/>.
/// </summary>
public sealed class TattoStudioDbContext : DbContext
{
    public TattoStudioDbContext(DbContextOptions<TattoStudioDbContext> options)
        : base(options) { }

    public DbSet<User>                 Users                 => Set<User>();
    public DbSet<Client>               Clients               => Set<Client>();
    public DbSet<Artist>               Artists               => Set<Artist>();
    public DbSet<Appointment>          Appointments          => Set<Appointment>();
    public DbSet<Consent>              Consents              => Set<Consent>();
    public DbSet<Payment>              Payments              => Set<Payment>();
    public DbSet<StockItem>            StockItems            => Set<StockItem>();
    public DbSet<ArtistCommissionsLog> ArtistCommissionsLogs => Set<ArtistCommissionsLog>();
    public DbSet<Invoice>              Invoices              => Set<Invoice>();
    public DbSet<GoogleOAuthToken>     GoogleOAuthTokens     => Set<GoogleOAuthToken>();
    public DbSet<AppointmentMedia>     AppointmentMedias     => Set<AppointmentMedia>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TattoStudioDbContext).Assembly);
    }
}
