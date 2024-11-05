using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Domain.Repositories;
using SMAIAXBackend.Infrastructure.DbContexts;

namespace SMAIAXBackend.Infrastructure.Repositories;

public class TenantRepository(ApplicationDbContext applicationDbContext) : ITenantRepository
{
    public TenantId NextIdentity()
    {
        return new TenantId(Guid.NewGuid());
    }

    public async Task AddAsync(Tenant tenant)
    {
        await applicationDbContext.Tenants.AddAsync(tenant);
        await applicationDbContext.SaveChangesAsync();
    }
}