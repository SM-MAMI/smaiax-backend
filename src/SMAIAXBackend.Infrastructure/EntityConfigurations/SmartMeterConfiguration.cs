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
    }
}