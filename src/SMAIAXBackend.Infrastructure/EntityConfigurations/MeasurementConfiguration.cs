using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.Infrastructure.EntityConfigurations;

public class MeasurementConfiguration : IEntityTypeConfiguration<Measurement>
{
    public void Configure(EntityTypeBuilder<Measurement> builder)
    {
        builder.ToTable("Measurement", "domain");

        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id)
            .HasConversion(
                v => v.Id,
                v => new MeasurementId(v))
            .IsRequired();

        builder.Property(m => m.Timestamp)
            .IsRequired();

        builder.OwnsOne(m => m.Data, data =>
        {
            data.Property(d => d.PositiveActivePower).HasColumnName("PositiveActivePower").IsRequired();
            data.Property(d => d.PositiveActiveEnergyTotal).HasColumnName("PositiveActiveEnergyTotal").IsRequired();
            data.Property(d => d.NegativeActivePower).HasColumnName("NegativeActivePower").IsRequired();
            data.Property(d => d.NegativeActiveEnergyTotal).HasColumnName("NegativeActiveEnergyTotal").IsRequired();
            data.Property(d => d.ReactiveEnergyQuadrant1Total).HasColumnName("ReactiveEnergyQuadrant1Total")
                .IsRequired();
            data.Property(d => d.ReactiveEnergyQuadrant3Total).HasColumnName("ReactiveEnergyQuadrant3Total")
                .IsRequired();
            data.Property(d => d.TotalPower).HasColumnName("TotalPower").IsRequired();
            data.Property(d => d.CurrentPhase1).HasColumnName("CurrentPhase1").IsRequired();
            data.Property(d => d.VoltagePhase1).HasColumnName("VoltagePhase1").IsRequired();
            data.Property(d => d.CurrentPhase2).HasColumnName("CurrentPhase2").IsRequired();
            data.Property(d => d.VoltagePhase2).HasColumnName("VoltagePhase2").IsRequired();
            data.Property(d => d.CurrentPhase3).HasColumnName("CurrentPhase3").IsRequired();
            data.Property(d => d.VoltagePhase3).HasColumnName("VoltagePhase3").IsRequired();
            data.Property(d => d.Uptime).HasColumnName("Uptime").IsRequired();
            data.Property(d => d.Timestamp).HasColumnName("Timestamp").IsRequired();
        });

        builder.Property(m => m.SmartMeterId)
            .HasConversion(
                v => v.Id,
                v => new SmartMeterId(v))
            .IsRequired();
    }
}