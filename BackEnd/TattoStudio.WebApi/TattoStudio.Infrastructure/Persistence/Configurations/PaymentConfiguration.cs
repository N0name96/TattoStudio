using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TattoStudio.Domain.Entities;

namespace TattoStudio.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración Fluent API de la entidad <see cref="Payment"/> para Supabase/PostgreSQL.
/// Define la tabla payments y la FK hacia appointments con DeleteBehavior.Restrict.
/// </summary>
public sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    /// <summary>Aplica el mapeo de columnas y relaciones al modelo EF Core.</summary>
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("payments");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
               .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(p => p.Amount)
               .IsRequired()
               .HasColumnType("numeric(10,2)");

        builder.Property(p => p.Type)
               .IsRequired()
               .HasColumnType("smallint");

        builder.Property(p => p.Method)
               .IsRequired()
               .HasColumnType("smallint");

        builder.Property(p => p.CreatedAt)
               .IsRequired()
               .HasColumnType("timestamp with time zone");

        builder.HasOne(p => p.Appointment)
               .WithMany()
               .HasForeignKey(p => p.AppointmentId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
