using Microsoft.EntityFrameworkCore;

using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Domain.Repositories;
using SMAIAXBackend.Infrastructure.DbContexts;

namespace SMAIAXBackend.Infrastructure.Repositories;

public class SmartMeterRepository(ApplicationDbContext applicationDbContext) : ISmartMeterRepository
{
    public SmartMeterId NextIdentity()
    {
        return new SmartMeterId(Guid.NewGuid());
    }

    public async Task AddAsync(SmartMeter meter)
    {
        await applicationDbContext.SmartMeters.AddAsync(meter);
        await applicationDbContext.SaveChangesAsync();
    }

    public Task<List<SmartMeter>> GetSmartMetersByUserIdAsync(UserId userId)
    {
        return applicationDbContext.SmartMeters
            .Where(sm => sm.UserId.Equals(userId))
            .Include(sm => sm.Metadata)
            .Include(sm => sm.Policies)
            .ToListAsync();
    }

    public Task<SmartMeter?> GetSmartMeterByIdAndUserIdAsync(SmartMeterId smartMeterId, UserId userId)
    {
        return applicationDbContext.SmartMeters
            .Include(sm => sm.Metadata)
            .Include(sm => sm.Policies)
            .FirstOrDefaultAsync(sm => sm.Id.Equals(smartMeterId) && sm.UserId.Equals(userId));
    }

    public async Task UpdateAsync(SmartMeter smartMeter)
    {
        applicationDbContext.SmartMeters.Update(smartMeter);
        await applicationDbContext.SaveChangesAsync();
    }
}