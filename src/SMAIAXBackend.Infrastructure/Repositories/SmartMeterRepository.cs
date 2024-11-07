using Microsoft.EntityFrameworkCore;

using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Domain.Repositories;
using SMAIAXBackend.Infrastructure.DbContexts;

namespace SMAIAXBackend.Infrastructure.Repositories;

public class SmartMeterRepository(ITenantDbContextFactory tenantDbContextFactory) : ISmartMeterRepository
{
    public SmartMeterId NextIdentity()
    {
        return new SmartMeterId(Guid.NewGuid());
    }

    public async Task AddAsync(SmartMeter meter, Tenant tenant)
    {
        var tenantDbContext =
            tenantDbContextFactory.CreateDbContext(tenant.DatabaseName, tenant.DatabaseUsername,
                tenant.DatabasePassword);
        await tenantDbContext.SmartMeters.AddAsync(meter);
        await tenantDbContext.SaveChangesAsync();
    }

    public Task<List<SmartMeter>> GetSmartMetersAsync(Tenant tenant)
    {
        var tenantDbContext =
            tenantDbContextFactory.CreateDbContext(tenant.DatabaseName, tenant.DatabaseUsername,
                tenant.DatabasePassword);
        return tenantDbContext.SmartMeters
            .Include(sm => sm.Metadata)
            .Include(sm => sm.Policies)
            .ToListAsync();
    }

    public Task<SmartMeter?> GetSmartMeterByIdAsync(SmartMeterId smartMeterId, Tenant tenant)
    {
        var tenantDbContext =
            tenantDbContextFactory.CreateDbContext(tenant.DatabaseName, tenant.DatabaseUsername,
                tenant.DatabasePassword);
        return tenantDbContext.SmartMeters
            .Include(sm => sm.Metadata)
            .Include(sm => sm.Policies)
            .FirstOrDefaultAsync(sm => sm.Id.Equals(smartMeterId));
    }

    public async Task UpdateAsync(SmartMeter smartMeter, Tenant tenant)
    {
        var tenantDbContext =
            tenantDbContextFactory.CreateDbContext(tenant.DatabaseName, tenant.DatabaseUsername,
                tenant.DatabasePassword);
        tenantDbContext.SmartMeters.Update(smartMeter);
        await tenantDbContext.SaveChangesAsync();
    }
}