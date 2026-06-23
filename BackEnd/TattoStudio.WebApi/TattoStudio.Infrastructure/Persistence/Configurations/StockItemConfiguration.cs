using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TattoStudio.Domain.Entities;

namespace TattoStudio.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración Fluent API de la entidad <see cref="StockItem"/> para Supabase/PostgreSQL.
/// Define la tabla stock_items e ignora la propiedad calculada RequiresRestock.
/// </summary>
public sealed class StockItemConfiguration : IEntityTypeConfiguration<StockItem>
{
    /// <summary>Aplica el mapeo de columnas al modelo EF Core.</summary>
    public void Configure(EntityTypeBuilder<StockItem> builder)
    {
        builder.ToTable("stock_items");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
               .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(s => s.Name)
               .IsRequired()
               .HasColumnType("varchar(150)");

        builder.Property(s => s.CurrentQuantity)
               .IsRequired()
               .HasColumnType("integer");

        builder.Property(s => s.MinThreshold)
               .IsRequired()
               .HasColumnType("integer");

        // RequiresRestock es una propiedad calculada: no se persiste en la base de datos
        builder.Ignore(s => s.RequiresRestock);
    }
}
