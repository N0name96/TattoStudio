using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TattoStudio.Domain.Entities;

namespace TattoStudio.Infraestructure.Persistence.Configurations;

public class StudioSettingsConfiguration : IEntityTypeConfiguration<StudioSettings>
{
    public void Configure(EntityTypeBuilder<StudioSettings> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.WorkdayStart).IsRequired();
        builder.Property(e => e.WorkdayEnd).IsRequired();
    }
}
