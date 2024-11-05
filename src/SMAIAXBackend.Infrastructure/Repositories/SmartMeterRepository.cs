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

    public async Task AddAsync(SmartMeter meter)
    {
        await tenantDbContext.SmartMeters.AddAsync(meter);
        await tenantDbContext.SaveChangesAsync();
    }

    public Task<List<SmartMeter>> GetSmartMetersByUserIdAsync(UserId userId)
    {
        return tenantDbContext.SmartMeters
            .Where(sm => sm.UserId.Equals(userId))
            .Include(sm => sm.Metadata)
            .Include(sm => sm.Policies)
            .ToListAsync();
    }

    public Task<SmartMeter?> GetSmartMeterByIdAndUserIdAsync(SmartMeterId smartMeterId, UserId userId)
    {
        return tenantDbContext.SmartMeters
            .Include(sm => sm.Metadata)
            .Include(sm => sm.Policies)
            .FirstOrDefaultAsync(sm => sm.Id.Equals(smartMeterId) && sm.UserId.Equals(userId));
    }

    public async Task UpdateAsync(SmartMeter smartMeter)
    {
        tenantDbContext.SmartMeters.Update(smartMeter);
        await tenantDbContext.SaveChangesAsync();
    }
}