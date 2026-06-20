using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TattoStudio.Domain.Entities;

namespace TattoStudio.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración Fluent API de la entidad <see cref="Client"/> para Supabase/PostgreSQL.
/// </summary>
public sealed class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.ToTable("clients");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
               .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(c => c.Name)
               .IsRequired()
               .HasMaxLength(150);

        builder.Property(c => c.Phone)
               .IsRequired()
               .HasMaxLength(20);

        builder.Property(c => c.Email)
               .IsRequired()
               .HasMaxLength(150);

        builder.HasIndex(c => c.Email)
               .IsUnique()
               .HasDatabaseName("ix_clients_email");

        builder.Property(c => c.BirthDate)
               .IsRequired()
               .HasColumnType("date");

        builder.Property(c => c.MedicalNotes)
               .HasColumnType("text");
    }
}
