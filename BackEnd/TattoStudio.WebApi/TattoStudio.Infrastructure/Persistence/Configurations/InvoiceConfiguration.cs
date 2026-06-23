using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TattoStudio.Domain.Entities;

namespace TattoStudio.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración Fluent API de la entidad <see cref="Invoice"/> para Supabase/PostgreSQL.
/// Define la tabla invoices con la cadena hash SHA-256 para VeriFactu (REG-06-01).
/// </summary>
public sealed class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    /// <summary>Aplica el mapeo de columnas y relaciones al modelo EF Core.</summary>
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("invoices");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Id)
               .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(i => i.InvoiceNumber)
               .IsRequired()
               .HasColumnType("varchar(50)");

        builder.HasIndex(i => i.InvoiceNumber)
               .IsUnique();

        builder.Property(i => i.AmountBase)
               .IsRequired()
               .HasColumnType("numeric(10,2)");

        builder.Property(i => i.TaxAmount)
               .IsRequired()
               .HasColumnType("numeric(10,2)");

        builder.Property(i => i.IssueDate)
               .IsRequired()
               .HasColumnType("timestamp with time zone");

        builder.Property(i => i.PreviousInvoiceHash)
               .IsRequired(false)
               .HasColumnType("varchar(64)")
               .HasDefaultValue(string.Empty);

        builder.Property(i => i.AeataStatus)
               .IsRequired()
               .HasColumnType("smallint");

        builder.HasOne(i => i.Payment)
               .WithMany()
               .HasForeignKey(i => i.PaymentId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
