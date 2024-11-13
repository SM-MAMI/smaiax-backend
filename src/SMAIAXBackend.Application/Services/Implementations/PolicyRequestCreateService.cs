using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Services.Interfaces;
using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects;
using SMAIAXBackend.Domain.Repositories;
using SMAIAXBackend.Domain.Specifications;

namespace SMAIAXBackend.Application.Services.Implementations;

public class PolicyRequestCreateService(
    IPolicyRequestRepository policyRequestRepository,
    ITenantRepository tenantRepository,
    IPolicyRepository policyRepository,
    ITenantContextService tenantContextService) : IPolicyRequestCreateService
{
    public async Task<List<PolicyDto>> CreatePolicyRequestAsync(PolicyRequestCreateDto policyRequestCreateDto)
    {
        var matchingPolices = new List<PolicyDto>();
        var policyRequestId = policyRequestRepository.NextIdentity();

        var locations = new List<Location>();
        foreach (var locationDto in policyRequestCreateDto.Locations)
        {
            var location = new Location(locationDto.StreetName, locationDto.City, locationDto.State,
                locationDto.Country, locationDto.Continent);
            locations.Add(location);
        }

        var policyFilter = new PolicyFilter(
            policyRequestCreateDto.MeasurementResolution,
            policyRequestCreateDto.MinHouseHoldSize,
            policyRequestCreateDto.MaxHouseHoldSize,
            locations,
            policyRequestCreateDto.LocationResolution,
            policyRequestCreateDto.MaxPrice);

        var policyRequest = PolicyRequest.Create(policyRequestId, policyRequestCreateDto.IsAutomaticContractingEnabled,
            policyFilter);
        await policyRequestRepository.AddAsync(policyRequest);

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
            matchingPolices.AddRange(from policy in policies where specification.IsSatisfiedBy(policy) select PolicyDto.FromPolicy(policy));
        }

        return matchingPolices;
    }
}