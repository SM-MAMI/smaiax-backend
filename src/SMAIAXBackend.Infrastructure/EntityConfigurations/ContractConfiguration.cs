using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.Infrastructure.EntityConfigurations;

public class ContractConfiguration : IEntityTypeConfiguration<Contract>
{
    public void Configure(EntityTypeBuilder<Contract> builder)
    {
        builder.ToTable("Contract", "domain");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
            .HasConversion(
                v => v.Id,
                v => new ContractId(v))
            .IsRequired();

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.PolicyId)
            .HasConversion(
                v => v.Id,
                v => new PolicyId(v))
            .IsRequired();

        builder.OwnsOne(c => c.PolicyCopy, policy =>
        {
            policy.Property(p => p.MeasurementResolution)
                .HasColumnName("MeasurementResolution")
                .HasConversion<string>()
                .IsRequired();

            policy.Property(p => p.HouseholdSize)
                .HasColumnName("HouseholdSize")
                .IsRequired();

            policy.OwnsOne(p => p.Location, location =>
            {
                location.Property(l => l.StreetName).HasColumnName("StreetName");
                location.Property(l => l.City).HasColumnName("City");
                location.Property(l => l.State).HasColumnName("State");
                location.Property(l => l.Country).HasColumnName("Country");
                location.Property(l => l.Continent)
                    .HasColumnName("Continent")
                    .HasConversion<string>();
            });

            policy.Property(p => p.LocationResolution)
                .HasColumnName("LocationResolution")
                .HasConversion<string>()
                .IsRequired();

            policy.Property(p => p.Price)
                .HasColumnName("Price")
                .IsRequired();

            policy.Property(p => p.UserId)
                .HasColumnName("UserId")
                .IsRequired();
        });

        builder.Property(c => c.PolicyRequestId)
            .HasConversion(
                v => v.Id,
                v => new PolicyRequestId(v))
            .IsRequired();

        builder.OwnsOne(c => c.PolicyRequestCopy, policyRequest =>
        {
            policyRequest.Property(p => p.IsAutomaticContractingEnabled)
                .HasColumnName("IsAutomaticContractingEnabled")
                .IsRequired();

            policyRequest.OwnsOne(p => p.PolicyFilter, policyFilter =>
            {
                policyFilter.Property(pf => pf.MeasurementResolution)
                    .HasColumnName("MeasurementResolution")
                    .HasConversion<string>()
                    .IsRequired();

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
                        .HasConversion<string>();
                });

                policyFilter.Property(pf => pf.LocationResolution)
                    .HasColumnName("LocationResolution")
                    .HasConversion<string>()
                    .IsRequired();

                policyFilter.Property(pf => pf.MaxPrice)
                    .HasColumnName("MaxPrice")
                    .IsRequired();
            });

            policyRequest.Property(p => p.UserId)
                .HasColumnName("UserId")
                .IsRequired();
        });
    }
}