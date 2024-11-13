using Microsoft.EntityFrameworkCore;

using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Domain.Repositories;
using SMAIAXBackend.Infrastructure.DbContexts;

namespace SMAIAXBackend.Infrastructure.Repositories;

public class PolicyRequestRepository(TenantDbContext tenantDbContext) : IPolicyRequestRepository
{
    public PolicyRequestId NextIdentity()
    {
        return new PolicyRequestId(Guid.NewGuid());
    }

    public async Task AddAsync(PolicyRequest policyRequest)
    {
        await tenantDbContext.PolicyRequests.AddAsync(policyRequest);
        await tenantDbContext.SaveChangesAsync();
    }

    public async Task<PolicyRequest?> GetPolicyRequestByIdAsync(PolicyRequestId id)
    {
        return await tenantDbContext.PolicyRequests
            .FirstOrDefaultAsync(pr => pr.Id.Equals(id));
    }
}