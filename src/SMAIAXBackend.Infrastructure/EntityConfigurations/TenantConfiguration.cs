using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.Infrastructure.EntityConfigurations;

public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("Tenant", "domain");
        
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id)
            .HasConversion(
                v => v.Id,
                v => new TenantId(v))
            .IsRequired();

        builder.Property(t => t.Name)
            .IsRequired();

        builder.Property(t => t.DatabaseConnectionString)
            .IsRequired();
    }
}