using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Domain.Repositories;
using SMAIAXBackend.Infrastructure.Configurations;
using SMAIAXBackend.Infrastructure.DbContexts;

namespace SMAIAXBackend.Infrastructure.Repositories;

public class PolicyRepository(
    TenantDbContext tenantDbContext,
    ITenantDbContextFactory tenantDbContextFactory,
    IOptions<DatabaseConfiguration> databaseConfigOptions) : IPolicyRepository
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

    public async Task<List<Policy>> GetPoliciesByTenantAsync(Tenant tenant)
    {
        var tenantSpecificDbContext = tenantDbContextFactory.CreateDbContext(tenant.DatabaseName,
            databaseConfigOptions.Value.SuperUsername, databaseConfigOptions.Value.SuperUserPassword);

        return await tenantSpecificDbContext.Policies.ToListAsync();
    }
}