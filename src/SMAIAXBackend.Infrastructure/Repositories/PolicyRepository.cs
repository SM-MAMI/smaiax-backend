using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Domain.Repositories;
using SMAIAXBackend.Infrastructure.DbContexts;

namespace SMAIAXBackend.Infrastructure.Repositories;

public class PolicyRepository(ApplicationDbContext applicationDbContext) : IPolicyRepository
{
    public PolicyId NextIdentity()
    {
        return new PolicyId(Guid.NewGuid());
    }

    public async Task AddAsync(Policy policy)
    {
        await applicationDbContext.Policies.AddAsync(policy);
        await applicationDbContext.SaveChangesAsync();
    }
}