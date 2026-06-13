using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TattoStudio.Domain.Entities;

namespace TattoStudio.Infraestructure.Persistence.Configurations;

public class ArtistConfiguration : IEntityTypeConfiguration<Artist>
{
    public void Configure(EntityTypeBuilder<Artist> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Surname).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Mail).IsRequired().HasMaxLength(150);
        builder.Property(e => e.PhoneNumber).HasMaxLength(20);
        builder.Property(e => e.Comision).HasPrecision(5, 2);

        builder.HasIndex(e => e.Mail).IsUnique();

        builder.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);
        builder.Property(e => e.DeactivatedAt).IsRequired(false);

        builder.HasOne<AppUser>()
               .WithMany()
               .HasForeignKey(e => e.UserId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
