using Microsoft.Extensions.Logging;

using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Exceptions;
using SMAIAXBackend.Application.Services.Interfaces;
using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Domain.Repositories;
using SMAIAXBackend.Domain.Specifications;

namespace SMAIAXBackend.Application.Services.Implementations;

public class PolicyMatchingService(
    IPolicyRequestRepository policyRequestRepository,
    ITenantRepository tenantRepository,
    IPolicyRepository policyRepository,
    ITenantContextService tenantContextService,
    ILogger<PolicyMatchingService> logger) : IPolicyMatchingService
{
    public async Task<List<PolicyDto>> GetMatchingPoliciesAsync(Guid policyRequestId)
    {
        var matchingPolices = new List<PolicyDto>();

        PolicyRequest? policyRequest =
            await policyRequestRepository.GetPolicyRequestByIdAsync(new PolicyRequestId(policyRequestId));

        if (policyRequest == null)
        {
            logger.LogWarning("Policy request with id {PolicyRequestId} not found", policyRequestId);
            throw new PolicyRequestNotFoundException(policyRequestId);
        }

        var currentTenant = await tenantContextService.GetCurrentTenantAsync();
        var tenants = await tenantRepository.GetAllAsync();
        var specification = new PolicyMatchesRequestSpecification(policyRequest);
        foreach (var tenant in tenants)
        {
            if (currentTenant.Equals(tenant))
            {
                continue;
            }

            var policies = await policyRepository.GetPoliciesByTenantAsync(tenant);
            matchingPolices.AddRange(from policy in policies
                                     where specification.IsSatisfiedBy(policy)
                                     select PolicyDto.FromPolicy(policy));
        }

        return matchingPolices;
    }
}