using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TattoStudio.Domain.Entities;

namespace TattoStudio.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración Fluent API de la entidad <see cref="AppointmentMedia"/> para Supabase/PostgreSQL.
/// Define la tabla appointment_medias con FK hacia appointments.
/// </summary>
public sealed class AppointmentMediaConfiguration : IEntityTypeConfiguration<AppointmentMedia>
{
    /// <summary>Aplica el mapeo de columnas y relaciones al modelo EF Core.</summary>
    public void Configure(EntityTypeBuilder<AppointmentMedia> builder)
    {
        builder.ToTable("appointment_medias");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id)
               .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(m => m.StorageUrl)
               .IsRequired()
               .HasColumnType("varchar(512)");

        builder.Property(m => m.StoragePath)
               .IsRequired()
               .HasColumnType("varchar(512)");

        builder.Property(m => m.MediaType)
               .IsRequired()
               .HasColumnType("smallint");

        builder.Property(m => m.CreatedAt)
               .IsRequired()
               .HasColumnType("timestamp with time zone");

        builder.HasOne(m => m.Appointment)
               .WithMany()
               .HasForeignKey(m => m.AppointmentId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
