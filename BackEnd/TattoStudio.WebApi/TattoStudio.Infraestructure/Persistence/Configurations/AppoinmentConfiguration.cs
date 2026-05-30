using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TattoStudio.Domain.Entities;

namespace TattoStudio.Infraestructure.Persistence.Configurations
{
    public class AppoinmentConfiguration : IEntityTypeConfiguration<Appoinment>
    {
        public void Configure(EntityTypeBuilder<Appoinment> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Name).IsRequired().HasMaxLength(100);
            builder.Property(e => e.MailClient).IsRequired().HasMaxLength(150);
            builder.Property(e => e.PhoneNumber).HasMaxLength(20);
            builder.Property(e => e.DepositAmount).HasPrecision(18, 2);
            builder.Property(e => e.TotalPrice).HasPrecision(18, 2);
        }
    }
}