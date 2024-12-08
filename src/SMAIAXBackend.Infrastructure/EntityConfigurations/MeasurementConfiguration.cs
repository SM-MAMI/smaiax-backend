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

        builder.HasNoKey();

        builder.Property(m => m.Timestamp)
            .IsRequired();
        builder.HasIndex(m => m.Timestamp).IsDescending(true);

        builder.Property(m => m.PositiveActivePower).HasColumnName("positiveActivePower").IsRequired();
        builder.Property(m => m.PositiveActiveEnergyTotal).HasColumnName("positiveActiveEnergyTotal").IsRequired();
        builder.Property(m => m.NegativeActivePower).HasColumnName("negativeActivePower").IsRequired();
        builder.Property(m => m.NegativeActiveEnergyTotal).HasColumnName("negativeActiveEnergyTotal").IsRequired();
        builder.Property(m => m.ReactiveEnergyQuadrant1Total).HasColumnName("reactiveEnergyQuadrant1Total")
            .IsRequired();
        builder.Property(m => m.ReactiveEnergyQuadrant3Total).HasColumnName("reactiveEnergyQuadrant3Total")
            .IsRequired();
        builder.Property(m => m.TotalPower).HasColumnName("totalPower").IsRequired();
        builder.Property(m => m.CurrentPhase1).HasColumnName("currentPhase1").IsRequired();
        builder.Property(m => m.VoltagePhase1).HasColumnName("voltagePhase1").IsRequired();
        builder.Property(m => m.CurrentPhase2).HasColumnName("currentPhase2").IsRequired();
        builder.Property(m => m.VoltagePhase2).HasColumnName("voltagePhase2").IsRequired();
        builder.Property(m => m.CurrentPhase3).HasColumnName("currentPhase3").IsRequired();
        builder.Property(m => m.VoltagePhase3).HasColumnName("voltagePhase3").IsRequired();
        builder.Property(m => m.Uptime).HasColumnName("uptime").IsRequired();
        builder.Property(m => m.Timestamp).HasColumnName("timestamp").IsRequired();

        builder.Property(m => m.SmartMeterId)
            .HasConversion(
                v => v.Id,
                v => new SmartMeterId(v))
            .IsRequired();
        builder.HasIndex(m => m.SmartMeterId);
    }
}