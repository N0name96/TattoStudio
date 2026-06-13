using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TattoStudio.Domain.Entities;

namespace TattoStudio.Infraestructure.Persistence.Configurations;

public class AppoinmentAuditLogConfiguration : IEntityTypeConfiguration<AppoinmentAuditLog>
{
    public void Configure(EntityTypeBuilder<AppoinmentAuditLog> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.FieldName).IsRequired().HasMaxLength(100);
        builder.Property(e => e.OldValue).HasMaxLength(500);
        builder.Property(e => e.NewValue).IsRequired().HasMaxLength(500);
        builder.Property(e => e.ChangedAt).IsRequired();

        builder.HasOne<Appoinment>()
            .WithMany()
            .HasForeignKey(e => e.AppoinmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<AppUser>()
            .WithMany()
            .HasForeignKey(e => e.ChangedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
