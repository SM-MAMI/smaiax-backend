using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Services.Interfaces;
using SMAIAXBackend.Domain.Model.Enums;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Domain.Repositories;

namespace SMAIAXBackend.Application.Services.Implementations;

public class PolicyListService(
    IPolicyRepository policyRepository,
    ITenantRepository tenantRepository,
    ITenantContextService tenantContextService) : IPolicyListService
{
    public async Task<List<PolicyDto>> GetPoliciesBySmartMeterIdAsync(SmartMeterId smartMeterId)
    {
        var policies = await policyRepository.GetPoliciesBySmartMeterIdAsync(smartMeterId);
        return policies.Select(PolicyDto.FromPolicy).ToList();
    }

    public async Task<List<PolicyDto>> GetPoliciesAsync()
    {
        var policies = await policyRepository.GetPoliciesAsync();
        return policies.Select(PolicyDto.FromPolicy).ToList();
    }

    public async Task<List<PolicyDto>> GetFilteredPoliciesAsync(decimal? maxPrice,
        MeasurementResolution? measurementResolution)
    {
        // TODO: Use Specification pattern to filter policies
        var matchingPolices = new List<PolicyDto>();

        var currentTenant = await tenantContextService.GetCurrentTenantAsync();
        var tenants = await tenantRepository.GetAllAsync();

        foreach (var tenant in tenants)
        {
            if (currentTenant.Equals(tenant))
            {
                continue;
            }
            
            var policies = await policyRepository.GetPoliciesByTenantAsync(tenant);
            
            foreach (var policy in policies)
            {
                if (maxPrice.HasValue && policy.Price > maxPrice)
                {
                    continue;
                }

                if (measurementResolution.HasValue && policy.MeasurementResolution != measurementResolution)
                {
                    continue;
                }

                matchingPolices.Add(PolicyDto.FromPolicy(policy));
            }
        }

        return matchingPolices;
    }
}