using Microsoft.EntityFrameworkCore;

using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Infrastructure.EntityConfigurations;

namespace SMAIAXBackend.Infrastructure.DbContexts;

public class TenantDbContext(DbContextOptions<TenantDbContext> options) : DbContext(options)
{
    public DbSet<SmartMeter> SmartMeters { get; init; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseCamelCaseNamingConvention();
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new MeasurementConfiguration());
        modelBuilder.ApplyConfiguration(new MetadataConfiguration());
        modelBuilder.ApplyConfiguration(new PolicyConfiguration());
        modelBuilder.ApplyConfiguration(new PolicyRequestConfiguration());
        modelBuilder.ApplyConfiguration(new SmartMeterConfiguration());
        modelBuilder.ApplyConfiguration(new PolicySmartMeterConfiguration());
    }
    
    public async Task SeedTestData()
    {
        var userId = new UserId(Guid.Parse("3c07065a-b964-44a9-9cdf-fbd49d755ea7"));
        var smartMeter1 = SmartMeter.Create(new SmartMeterId(Guid.NewGuid()), "Smart Meter 1", userId);
        var smartMeter2 = SmartMeter.Create(new SmartMeterId(Guid.NewGuid()), "Smart Meter 2", userId);
        
        await SmartMeters.AddAsync(smartMeter1);
        await SmartMeters.AddAsync(smartMeter2);
        await SaveChangesAsync();
    }
}