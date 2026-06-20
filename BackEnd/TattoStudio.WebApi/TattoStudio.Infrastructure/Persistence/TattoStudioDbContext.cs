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

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TattoStudioDbContext).Assembly);
    }
}
