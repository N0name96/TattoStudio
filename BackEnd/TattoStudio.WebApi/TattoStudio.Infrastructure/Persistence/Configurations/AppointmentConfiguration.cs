using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TattoStudio.Domain.Entities;

namespace TattoStudio.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración Fluent API de la entidad <see cref="Appointment"/> para Supabase/PostgreSQL.
/// Define la tabla appointments, las FK con DeleteBehavior.Restrict y el índice compuesto
/// (ArtistId, DateTime, Status) requerido por el spec §2.
/// </summary>
public sealed class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    /// <summary>Aplica el mapeo de columnas, relaciones e índices al modelo EF Core.</summary>
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("appointments");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
               .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(a => a.DateTime)
               .IsRequired()
               .HasColumnType("timestamp with time zone");

        builder.Property(a => a.DurationHours)
               .IsRequired()
               .HasColumnType("integer");

        builder.Property(a => a.Status)
               .IsRequired()
               .HasColumnType("smallint");

        builder.Property(a => a.HasDeposit)
               .IsRequired()
               .HasDefaultValue(false);

        builder.Property(a => a.DepositAmount)
               .HasColumnType("numeric(10,2)")
               .HasDefaultValue(0m);

        builder.HasOne(a => a.Client)
               .WithMany()
               .HasForeignKey(a => a.ClientId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Artist)
               .WithMany()
               .HasForeignKey(a => a.ArtistId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(a => new { a.ArtistId, a.DateTime, a.Status })
               .HasDatabaseName("ix_appointments_artist_datetime_status");
    }
}
