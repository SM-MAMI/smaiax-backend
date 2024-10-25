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
            location.Property(l => l.StreetName).HasColumnName("streetName");
            location.Property(l => l.City).HasColumnName("city");
            location.Property(l => l.State).HasColumnName("state");
            location.Property(l => l.Country).HasColumnName("country");
            location.Property(l => l.Continent).HasColumnName("continent").HasConversion<string>();
        });

        builder.Property(p => p.LocationResolution).HasConversion<string>().IsRequired();

        builder.Property(p => p.Price).IsRequired();

        builder.Property(p => p.UserId)
            .HasConversion(
                v => v.Id,
                v => new UserId(v))
            .IsRequired();

        builder.Property(p => p.State).HasConversion<string>().IsRequired();
    }
}