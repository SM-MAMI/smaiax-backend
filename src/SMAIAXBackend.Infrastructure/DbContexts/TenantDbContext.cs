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
    public DbSet<Measurement> Measurements { get; init; }
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
        SmartMeterId smartMeter1Id = new(Guid.NewGuid());
        Metadata metadata = Metadata.Create(new MetadataId(Guid.NewGuid()), DateTime.UtcNow,
            new Location("Hochschulstraße 1", "Dornbirn", "Vorarlberg", "Österreich", Continent.Oceania),
            4, smartMeter1Id);
        SmartMeter smartMeter1 = SmartMeter.Create(smartMeter1Id, "Smart Meter 1", [metadata]);
        SmartMeter smartMeter2 = SmartMeter.Create(new SmartMeterId(Guid.NewGuid()), "Smart Meter 2", []);

        await SmartMeters.AddAsync(smartMeter1);
        await SmartMeters.AddAsync(smartMeter2);
        // Can't be inserted via "AddAsync".
        await Database.ExecuteSqlRawAsync(
            $@"INSERT INTO domain.""Measurement"" VALUES 
                                       (160, 1137778, 0, 0, 3837, 717727, 160, 1.03, 229.80, 0.42, 229.00, 0.17, 229.60,
                                        '0000:01:49:41', '{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}', '{smartMeter1Id}');");
        await SaveChangesAsync();
    }
}