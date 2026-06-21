using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TattoStudio.Domain.Entities;

namespace TattoStudio.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración Fluent API de la entidad <see cref="Consent"/> para Supabase/PostgreSQL.
/// Define la tabla consents, la relación 1:1 con appointments y las restricciones de columna.
/// </summary>
public sealed class ConsentConfiguration : IEntityTypeConfiguration<Consent>
{
    /// <summary>Aplica el mapeo de columnas, relaciones e índices al modelo EF Core.</summary>
    public void Configure(EntityTypeBuilder<Consent> builder)
    {
        builder.ToTable("consents");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
               .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(c => c.IsSigned)
               .IsRequired()
               .HasDefaultValue(false);

        builder.Property(c => c.SignedAt)
               .HasColumnType("timestamp with time zone")
               .IsRequired(false);

        builder.Property(c => c.SignatureBase64)
               .HasColumnType("text")
               .IsRequired(false);

        builder.HasOne(c => c.Appointment)
               .WithOne()
               .HasForeignKey<Consent>(c => c.AppointmentId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(c => c.AppointmentId)
               .IsUnique()
               .HasDatabaseName("ix_consents_appointment_id");

        builder.HasOne<Client>()
               .WithMany()
               .HasForeignKey(c => c.ClientId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
