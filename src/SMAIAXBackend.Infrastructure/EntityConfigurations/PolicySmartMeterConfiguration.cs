using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.RelationshipHelpers;

namespace SMAIAXBackend.Infrastructure.EntityConfigurations;

public class PolicySmartMeterConfiguration : IEntityTypeConfiguration<PolicySmartMeter>
{
    public void Configure(EntityTypeBuilder<PolicySmartMeter> builder)
    {
        builder.ToTable("PolicySmartMeter", "domain");

        builder.HasKey(e => new { e.PolicyId, e.SmartMeterId });

        builder
            .HasOne<Policy>()
            .WithMany(p => p.SmartMeters)
            .HasForeignKey(psm => psm.PolicyId)
            .HasPrincipalKey(p => p.Id)
            .IsRequired();

        builder
            .HasOne<SmartMeter>()
            .WithMany(sm => sm.Policies)
            .HasForeignKey(psm => psm.SmartMeterId)
            .HasPrincipalKey(sm => sm.Id)
            .IsRequired();
    }
}