using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.Infrastructure.EntityConfigurations;

public class DomainUserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("DomainUser", "domain");

        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id)
            .HasConversion(
                v => v.Id,
                v => new UserId(v))
            .IsRequired();

        builder.OwnsOne(u => u.Name, name =>
        {
            name.Property(fn => fn.FirstName).HasColumnName("firstName").IsRequired();
            name.Property(fn => fn.LastName).HasColumnName("lastName").IsRequired();
        });

        builder.Property(u => u.Email)
            .IsRequired();

        builder.Property(u => u.TenantId)
            .HasConversion(
                v => v.Id,
                v => new TenantId(v))
            .IsRequired();
    }
}