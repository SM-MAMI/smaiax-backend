using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects;

namespace SMAIAXBackend.Infrastructure.EntityConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("User");
        
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
        
        builder.OwnsOne(u => u.Address, name =>
        {
            name.Property(a => a.Street).HasColumnName("street").IsRequired();
            name.Property(a => a.City).HasColumnName("city").IsRequired();
            name.Property(a => a.State).HasColumnName("state").IsRequired();
            name.Property(a => a.ZipCode).HasColumnName("zipCode").IsRequired();
            name.Property(a => a.Country).HasColumnName("country").IsRequired();
        });
        
        builder.Property(u => u.Email)
            .IsRequired();
    }
}