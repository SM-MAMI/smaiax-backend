using Microsoft.EntityFrameworkCore;

using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.Enums;
using SMAIAXBackend.Domain.Model.ValueObjects;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Infrastructure.EntityConfigurations;

namespace SMAIAXBackend.Infrastructure.DbContexts;

public class TenantDbContext(DbContextOptions<TenantDbContext> options) : DbContext(options)
{
    public DbSet<SmartMeter> SmartMeters { get; init; }
    public DbSet<Policy> Policies { get; init; }
    public DbSet<PolicyRequest> PolicyRequests { get; init; }

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
    }

    public async Task SeedTestData()
    {
        var smartMeter1Id = new SmartMeterId(Guid.NewGuid());
        var metadata = Metadata.Create(new MetadataId(Guid.NewGuid()), DateTime.UtcNow,
            new Location("Hochschulstraße 1", "Dornbirn", "Vorarlberg", "Österreich", Continent.Oceania),
            4, smartMeter1Id);
        var smartMeter1 = SmartMeter.Create(smartMeter1Id, "Smart Meter 1", [metadata]);
        var smartMeter2 = SmartMeter.Create(new SmartMeterId(Guid.NewGuid()), "Smart Meter 2", []);

        await SmartMeters.AddAsync(smartMeter1);
        await SmartMeters.AddAsync(smartMeter2);
        await SaveChangesAsync();
    }
}