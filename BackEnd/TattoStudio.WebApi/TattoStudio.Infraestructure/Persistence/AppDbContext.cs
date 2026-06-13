using Microsoft.EntityFrameworkCore;
using TattoStudio.Domain.Entities;

namespace TattoStudio.Infraestructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Appoinment> Appoinments { get; set; }
        public DbSet<AppoinmentAuditLog> AppoinmentAuditLogs { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<AppUser> Users { get; set; }
        public DbSet<StudioSettings> StudioSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}