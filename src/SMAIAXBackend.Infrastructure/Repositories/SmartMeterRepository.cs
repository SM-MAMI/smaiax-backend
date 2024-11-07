using Microsoft.EntityFrameworkCore;

using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Domain.Repositories;
using SMAIAXBackend.Infrastructure.DbContexts;

namespace SMAIAXBackend.Infrastructure.Repositories;

public class SmartMeterRepository(TenantDbContext tenantDbContext) : ISmartMeterRepository
{
    public SmartMeterId NextIdentity()
    {
        return new SmartMeterId(Guid.NewGuid());
    }
    
    public MetadataId NextMetadataIdentity()
    {
        return new MetadataId(Guid.NewGuid());
    }
    
    public async Task AddAsync(SmartMeter meter)
    {
        await tenantDbContext.SmartMeters.AddAsync(meter);
        await tenantDbContext.SaveChangesAsync();
    }

    public async Task<List<SmartMeter>> GetSmartMetersAsync()
    {
        return await tenantDbContext.SmartMeters
            .Include(sm => sm.Metadata)
            .Include(sm => sm.Policies)
            .ToListAsync();
    }

    public async Task<SmartMeter?> GetSmartMeterByIdAsync(SmartMeterId smartMeterId)
    {
        return await tenantDbContext.SmartMeters
            .Include(sm => sm.Metadata)
            .Include(sm => sm.Policies)
            .FirstOrDefaultAsync(sm => sm.Id.Equals(smartMeterId));
    }

    public async Task UpdateAsync(SmartMeter smartMeter)
    {
        tenantDbContext.SmartMeters.Update(smartMeter);
        await tenantDbContext.SaveChangesAsync();
    }
}