using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TattoStudio.Domain.Entities;

namespace TattoStudio.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración Fluent API de la entidad <see cref="ArtistCommissionsLog"/> para Supabase/PostgreSQL.
/// Define la tabla artist_commissions_log y las FK con DeleteBehavior.Restrict.
/// </summary>
public sealed class ArtistCommissionsLogConfiguration : IEntityTypeConfiguration<ArtistCommissionsLog>
{
    /// <summary>Aplica el mapeo de columnas y relaciones al modelo EF Core.</summary>
    public void Configure(EntityTypeBuilder<ArtistCommissionsLog> builder)
    {
        builder.ToTable("artist_commissions_log");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
               .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(a => a.AmountOwed)
               .IsRequired()
               .HasColumnType("numeric(10,2)");

        builder.Property(a => a.CreatedAt)
               .IsRequired()
               .HasColumnType("timestamp with time zone");

        builder.HasOne<Artist>()
               .WithMany()
               .HasForeignKey(a => a.ArtistId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Appointment>()
               .WithMany()
               .HasForeignKey(a => a.AppointmentId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
