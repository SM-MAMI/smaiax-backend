using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.Infrastructure.EntityConfigurations;

public class PolicyRequestConfiguration : IEntityTypeConfiguration<PolicyRequest>
{
    public void Configure(EntityTypeBuilder<PolicyRequest> builder)
    {
        builder.ToTable("PolicyRequest", "domain");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .HasConversion(
                v => v.Id,
                v => new PolicyRequestId(v))
            .IsRequired();

        builder.Property(p => p.IsAutomaticContractingEnabled).IsRequired();

        builder.OwnsOne(p => p.PolicyFilter, policyFilter =>
        {
            policyFilter.Property(pf => pf.MeasurementResolution).HasColumnName("MeasurementResolution")
                .HasConversion<string>().IsRequired();

            policyFilter.Property(pf => pf.MinHouseHoldSize).HasColumnName("MinHouseHoldSize").IsRequired();

            policyFilter.Property(pf => pf.MaxHouseHoldSize).HasColumnName("MaxHouseHoldSize").IsRequired();

            policyFilter.OwnsMany(pf => pf.Locations, location =>
            {
                location.Property(l => l.StreetName).HasColumnName("StreetName");
                location.Property(l => l.City).HasColumnName("City");
                location.Property(l => l.State).HasColumnName("State");
                location.Property(l => l.Country).HasColumnName("Country");
                location.Property(l => l.Continent)
                    .HasColumnName("Continent")
                    .HasConversion<string>()
                    .IsRequired();

                location.WithOwner();
            });

            policyFilter.Property(pf => pf.LocationResolution).HasColumnName("LocationResolution")
                .HasConversion<string>().IsRequired();

            policyFilter.Property(pf => pf.MaxPrice).HasColumnName("MaxPrice").IsRequired();
        });

        builder.Property(p => p.UserId)
            .HasConversion(
                v => v.Id,
                v => new UserId(v))
            .IsRequired();
    }
}