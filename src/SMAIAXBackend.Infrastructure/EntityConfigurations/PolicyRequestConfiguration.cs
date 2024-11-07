using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Newtonsoft.Json;

using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects;
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
            policyFilter.Property(pf => pf.MeasurementResolution).HasColumnName("measurementResolution")
                .HasConversion<string>().IsRequired();

            policyFilter.Property(pf => pf.MinHouseHoldSize).HasColumnName("minHouseHoldSize").IsRequired();

            policyFilter.Property(pf => pf.MaxHouseHoldSize).HasColumnName("maxHouseHoldSize").IsRequired();

            policyFilter.Property(pf => pf.Locations).HasColumnName("locations").HasConversion<string>(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<List<Location>>(v)!
            );

            policyFilter.Property(pf => pf.LocationResolution).HasColumnName("locationResolution")
                .HasConversion<string>().IsRequired();

            policyFilter.Property(pf => pf.MaxPrice).HasColumnName("maxPrice").IsRequired();
        });

        builder.Property(p => p.State).HasConversion<string>().IsRequired();

        builder.Property(p => p.UserId)
            .HasConversion(
                v => v.Id,
                v => new UserId(v))
            .IsRequired();
    }
}