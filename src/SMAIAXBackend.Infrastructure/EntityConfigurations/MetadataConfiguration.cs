using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.Infrastructure.EntityConfigurations;

public class MetadataConfiguration : IEntityTypeConfiguration<Metadata>
{
    public void Configure(EntityTypeBuilder<Metadata> builder)
    {
        builder.ToTable("Metadata", "domain");

        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id)
            .HasConversion(
                v => v.Id,
                v => new MetadataId(v))
            .IsRequired();

        builder.Property(m => m.ValidFrom)
            .IsRequired();
        builder.HasIndex(m => m.ValidFrom)
            .IsUnique();

        builder.OwnsOne(m => m.Location, location =>
        {
            location.Property(l => l.StreetName).HasColumnName("streetName").HasMaxLength(200);
            location.Property(l => l.City).HasColumnName("city").HasMaxLength(200);
            location.Property(l => l.State).HasColumnName("state").HasMaxLength(200);
            location.Property(l => l.Country).HasColumnName("country").HasMaxLength(200);
            location.Property(l => l.Continent).HasColumnName("continent").HasConversion<string>().HasMaxLength(200);
        });

        builder.Property(m => m.HouseholdSize);

        builder.HasOne(md => md.SmartMeter)
            .WithMany(sm => sm.Metadata)
            .HasForeignKey(md => md.SmartMeterId)
            .IsRequired();
    }
}