using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TattoStudio.Domain.Entities;

namespace TattoStudio.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración Fluent API de la entidad <see cref="Artist"/> para Supabase/PostgreSQL.
/// </summary>
public sealed class ArtistConfiguration : IEntityTypeConfiguration<Artist>
{
    public void Configure(EntityTypeBuilder<Artist> builder)
    {
        builder.ToTable("artists");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
               .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(a => a.Name)
               .IsRequired()
               .HasMaxLength(150);

        builder.Property(a => a.Specialty)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(a => a.CommissionPercentage)
               .IsRequired()
               .HasColumnType("numeric(5,2)");
    }
}
