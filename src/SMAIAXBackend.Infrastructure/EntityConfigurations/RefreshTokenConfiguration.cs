using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects;

namespace SMAIAXBackend.Infrastructure.EntityConfigurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshToken");

        builder.HasKey(rt => rt.Id);
        builder.Property(rt => rt.Id)
            .HasConversion(
                v => v.Id,
                v => new RefreshTokenId(v))
            .IsRequired();

        builder.Property(rt => rt.UserId)
            .HasConversion(
                v => v.Id,
                v => new UserId(v))
            .IsRequired();

        builder.Property(rt => rt.JwtTokenId)
            .IsRequired();

        builder.Property(rt => rt.Token)
            .IsRequired();

        builder.Property(rt => rt.IsValid)
            .IsRequired();

        builder.Property(rt => rt.ExpiresAt)
            .IsRequired();
    }
}