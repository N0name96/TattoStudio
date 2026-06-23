using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TattoStudio.Domain.Entities;

namespace TattoStudio.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración Fluent API de la entidad <see cref="GoogleOAuthToken"/> para Supabase/PostgreSQL.
/// Define la tabla google_oauth_tokens con FK hacia usuarios.
/// </summary>
public sealed class GoogleOAuthTokenConfiguration : IEntityTypeConfiguration<GoogleOAuthToken>
{
    /// <summary>Aplica el mapeo de columnas y relaciones al modelo EF Core.</summary>
    public void Configure(EntityTypeBuilder<GoogleOAuthToken> builder)
    {
        builder.ToTable("google_oauth_tokens");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.AccessToken)
               .IsRequired()
               .HasColumnType("text");

        builder.Property(t => t.RefreshToken)
               .IsRequired()
               .HasColumnType("text");

        builder.Property(t => t.ExpiresAt)
               .IsRequired()
               .HasColumnType("timestamp with time zone");

        builder.HasOne<User>()
               .WithMany()
               .HasForeignKey(t => t.UserId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
