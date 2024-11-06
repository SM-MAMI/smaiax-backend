using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Services.Interfaces;
using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects;
using SMAIAXBackend.Domain.Repositories;

namespace SMAIAXBackend.Application.Services.Implementations;

public class PolicyCreateService(IPolicyRepository policyRepository, IUserValidationService userValidationService)
    : IPolicyCreateService
{
    public async Task<Guid> CreatePolicyAsync(PolicyCreateDto policyCreateDto, string? userId)
    {
        // TODO: Validate Dto
        var validatedUserId = await userValidationService.ValidateUserAsync(userId);
        var policyId = policyRepository.NextIdentity();
        var location = new Location(policyCreateDto.Location.StreetName, policyCreateDto.Location.City,
            policyCreateDto.Location.State, policyCreateDto.Location.Country, policyCreateDto.Location.Continent);
        var policy = Policy.Create(policyId, policyCreateDto.MeasurementResolution, policyCreateDto.HouseholdSize,
            location, policyCreateDto.LocationResolution, policyCreateDto.Price, validatedUserId);

        await policyRepository.AddAsync(policy);

        return policyId.Id;
    }
}