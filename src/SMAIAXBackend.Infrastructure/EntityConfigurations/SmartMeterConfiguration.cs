using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.Infrastructure.EntityConfigurations;

public class SmartMeterConfiguration : IEntityTypeConfiguration<SmartMeter>
{
    public void Configure(EntityTypeBuilder<SmartMeter> builder)
    {
        builder.ToTable("SmartMeter", "domain");

        builder.HasKey(sm => sm.Id);
        builder.Property(sm => sm.Id)
            .HasConversion(
                v => v.Id,
                v => new SmartMeterId(v))
            .IsRequired();

        builder.Property(sm => sm.Name).IsRequired();

        builder.HasMany(sm => sm.Metadata)
            .WithOne(m => m.SmartMeter)
            .HasForeignKey(m => m.SmartMeterId)
            .IsRequired();

        builder.Property(sm => sm.UserId)
            .HasConversion(
                v => v.Id,
                v => new UserId(v))
            .IsRequired();

        builder.HasMany<Policy>()
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "PolicySmartMeters",
                j => j.HasOne<Policy>()
                    .WithMany()
                    .HasForeignKey("PolicyId")
                    .OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<SmartMeter>()
                    .WithMany()
                    .HasForeignKey("SmartMeterId")
                    .OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.HasKey("PolicyId", "SmartMeterId");
                    j.ToTable("PolicySmartMeters");
                });

        builder.HasMany(sm => sm.Metadata)
            .WithOne(md => md.SmartMeter)
            .HasForeignKey(md => md.SmartMeterId)
            .IsRequired();
    }
}