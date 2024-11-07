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
            data.Property(d => d.PositiveActivePower).HasColumnName("positiveActivePower").IsRequired();
            data.Property(d => d.PositiveActiveEnergyTotal).HasColumnName("positiveActiveEnergyTotal").IsRequired();
            data.Property(d => d.NegativeActivePower).HasColumnName("negativeActivePower").IsRequired();
            data.Property(d => d.NegativeActiveEnergyTotal).HasColumnName("negativeActiveEnergyTotal").IsRequired();
            data.Property(d => d.ReactiveEnergyQuadrant1Total).HasColumnName("reactiveEnergyQuadrant1Total")
                .IsRequired();
            data.Property(d => d.ReactiveEnergyQuadrant3Total).HasColumnName("reactiveEnergyQuadrant3Total")
                .IsRequired();
            data.Property(d => d.TotalPower).HasColumnName("totalPower").IsRequired();
            data.Property(d => d.CurrentPhase1).HasColumnName("currentPhase1").IsRequired();
            data.Property(d => d.VoltagePhase1).HasColumnName("voltagePhase1").IsRequired();
            data.Property(d => d.CurrentPhase2).HasColumnName("currentPhase2").IsRequired();
            data.Property(d => d.VoltagePhase2).HasColumnName("voltagePhase2").IsRequired();
            data.Property(d => d.CurrentPhase3).HasColumnName("currentPhase3").IsRequired();
            data.Property(d => d.VoltagePhase3).HasColumnName("voltagePhase3").IsRequired();
            data.Property(d => d.Uptime).HasColumnName("uptime").IsRequired();
            data.Property(d => d.Timestamp).HasColumnName("timestamp").IsRequired();
        });

        builder.Property(m => m.SmartMeterId)
            .HasConversion(
                v => v.Id,
                v => new SmartMeterId(v))
            .IsRequired();
    }
}