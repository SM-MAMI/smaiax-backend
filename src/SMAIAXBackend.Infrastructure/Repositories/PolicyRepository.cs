using Microsoft.EntityFrameworkCore;

using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Domain.Repositories;
using SMAIAXBackend.Infrastructure.DbContexts;

namespace SMAIAXBackend.Infrastructure.Repositories;

public class PolicyRepository(TenantDbContext tenantDbContext) : IPolicyRepository
{
    public PolicyId NextIdentity()
    {
        return new PolicyId(Guid.NewGuid());
    }

    public async Task AddAsync(Policy policy)
    {
        await tenantDbContext.Policies.AddAsync(policy);
        await tenantDbContext.SaveChangesAsync();
    }

    public Task<List<Policy>> GetPoliciesBySmartMeterIdAsync(SmartMeterId smartMeterId)
    {
        return tenantDbContext.Policies
            .Where(p => p.SmartMeterId.Equals(smartMeterId))
            .ToListAsync();
    }
}