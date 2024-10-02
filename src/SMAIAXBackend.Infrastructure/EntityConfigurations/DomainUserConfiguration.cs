using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects;

namespace SMAIAXBackend.Infrastructure.EntityConfigurations;

public class DomainUserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("DomainUser");
        
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
        
        builder.OwnsOne(u => u.Address, address =>
        {
            address.Property(a => a.Street).HasColumnName("street").IsRequired();
            address.Property(a => a.City).HasColumnName("city").IsRequired();
            address.Property(a => a.State).HasColumnName("state").IsRequired();
            address.Property(a => a.ZipCode).HasColumnName("zipCode").IsRequired();
            address.Property(a => a.Country).HasColumnName("country").IsRequired();
        });
        
        builder.Property(u => u.Email)
            .IsRequired();
    }
}