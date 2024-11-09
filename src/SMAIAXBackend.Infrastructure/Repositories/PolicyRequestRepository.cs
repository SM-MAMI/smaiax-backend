using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Domain.Repositories;
using SMAIAXBackend.Infrastructure.DbContexts;

namespace SMAIAXBackend.Infrastructure.Repositories;

public class PolicyRequestRepository(ApplicationDbContext applicationDbContext) : IPolicyRequestRepository
{
    public PolicyRequestId NextIdentity()
    {
        return new PolicyRequestId(Guid.NewGuid());
    }

    public async Task AddAsync(PolicyRequest policyRequest)
    {
        await applicationDbContext.PolicyRequests.AddAsync(policyRequest);
        await applicationDbContext.SaveChangesAsync();
    }
}