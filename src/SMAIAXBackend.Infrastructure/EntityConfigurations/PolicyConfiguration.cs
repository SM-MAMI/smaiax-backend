using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.Infrastructure.EntityConfigurations;

public class PolicyConfiguration : IEntityTypeConfiguration<Policy>
{
    public void Configure(EntityTypeBuilder<Policy> builder)
    {
        builder.ToTable("Policy", "domain");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .HasConversion(
                v => v.Id,
                v => new PolicyId(v))
            .IsRequired();

        builder.Property(p => p.MeasurementResolution).HasConversion<string>().IsRequired();

        builder.Property(p => p.HouseholdSize).IsRequired();

        builder.OwnsOne(p => p.Location, location =>
        {
            location.Property(l => l.StreetName).HasColumnName("StreetName");
            location.Property(l => l.City).HasColumnName("City");
            location.Property(l => l.State).HasColumnName("State");
            location.Property(l => l.Country).HasColumnName("Country");
            location.Property(l => l.Continent).HasColumnName("Continent").HasConversion<string>();
        });

        builder.Property(p => p.LocationResolution).HasConversion<string>().IsRequired();

        builder.Property(p => p.Price).IsRequired();

        builder.Property(p => p.UserId)
            .HasConversion(
                v => v.Id,
                v => new UserId(v))
            .IsRequired();

        builder.Property(p => p.State).HasConversion<string>().IsRequired();

        builder.HasMany<SmartMeter>()
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "PolicySmartMeters",
                j => j.HasOne<SmartMeter>()
                    .WithMany()
                    .HasForeignKey("SmartMeterId")
                    .OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<Policy>()
                    .WithMany()
                    .HasForeignKey("PolicyId")
                    .OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.HasKey("PolicyId", "SmartMeterId");
                    j.ToTable("PolicySmartMeters");
                });
    }
}