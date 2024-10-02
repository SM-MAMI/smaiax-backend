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
            name.Property(fn => fn.FirstName).HasColumnName("FirstName").IsRequired();
            name.Property(fn => fn.LastName).HasColumnName("LastName").IsRequired();
        });
        
        builder.OwnsOne(u => u.Address, address =>
        {
            address.Property(a => a.Street).HasColumnName("Street").IsRequired();
            address.Property(a => a.City).HasColumnName("City").IsRequired();
            address.Property(a => a.State).HasColumnName("State").IsRequired();
            address.Property(a => a.ZipCode).HasColumnName("ZipCode").IsRequired();
            address.Property(a => a.Country).HasColumnName("Country").IsRequired();
        });
        
        builder.Property(u => u.Email)
            .IsRequired();
    }
}