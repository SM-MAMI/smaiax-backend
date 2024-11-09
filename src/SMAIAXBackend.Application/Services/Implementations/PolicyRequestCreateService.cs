using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Services.Interfaces;
using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects;
using SMAIAXBackend.Domain.Repositories;

namespace SMAIAXBackend.Application.Services.Implementations;

public class PolicyRequestCreateService(
    IPolicyRequestRepository policyRequestRepository,
    IUserValidationService userValidationService) : IPolicyRequestCreateService
{
    public async Task<Guid> CreatePolicyRequestAsync(PolicyRequestCreateDto policyRequestCreateDto, string? userId)
    {
        var validatedUserId = await userValidationService.ValidateUserAsync(userId);
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
            policyFilter, validatedUserId);

        await policyRequestRepository.AddAsync(policyRequest);

        return policyRequestId.Id;
    }
}