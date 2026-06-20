using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TattoStudio.Domain.Entities;
using TattoStudio.Domain.Enums;

namespace TattoStudio.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración Fluent API de la entidad <see cref="User"/> para Supabase/PostgreSQL.
/// Define tabla, columnas varchar, índice único en email y conversión de Role a smallint.
/// </summary>
public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(u => u.Id);

        // Guid generado en Postgres con gen_random_uuid() (spec §3)
        builder.Property(u => u.Id)
               .HasDefaultValueSql("gen_random_uuid()");

        // Email: varchar(150), único, índice B-Tree (spec §3 + REG-01-01)
        builder.Property(u => u.Email)
               .IsRequired()
               .HasMaxLength(150);

        builder.HasIndex(u => u.Email)
               .IsUnique()
               .HasDatabaseName("ix_users_email");

        builder.Property(u => u.PasswordHash)
               .IsRequired()
               .HasMaxLength(255);

        builder.Property(u => u.Name)
               .IsRequired()
               .HasMaxLength(100);

        // Role almacenado como smallint en Postgres (spec §3)
        builder.Property(u => u.Role)
               .HasConversion<short>()
               .HasColumnType("smallint");
    }
}
