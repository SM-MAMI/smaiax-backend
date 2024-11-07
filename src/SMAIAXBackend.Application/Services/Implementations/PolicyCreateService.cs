using Microsoft.Extensions.Logging;

using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Exceptions;
using SMAIAXBackend.Application.Services.Interfaces;
using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Domain.Repositories;

namespace SMAIAXBackend.Application.Services.Implementations;

public class PolicyCreateService(
    IPolicyRepository policyRepository,
    ISmartMeterRepository smartMeterRepository,
    IUserValidationService userValidationService,
    ILogger<PolicyCreateService> logger)
    : IPolicyCreateService
{
    public async Task<Guid> CreatePolicyAsync(PolicyCreateDto policyCreateDto, string? userId)
    {
        var validatedUserId = await userValidationService.ValidateUserAsync(userId);

        var smartMeter = await smartMeterRepository.GetSmartMeterByIdAndUserIdAsync(
            new SmartMeterId(policyCreateDto.SmartMeterId),
            validatedUserId);

        if (smartMeter == null)
        {
            logger.LogWarning("SmartMeter with id {SmartMeterId} not found for user {UserId}",
                policyCreateDto.SmartMeterId, validatedUserId.Id);
            throw new SmartMeterNotFoundException(policyCreateDto.SmartMeterId, validatedUserId.Id);
        }
        
        if (!smartMeter.UserId.Equals(validatedUserId))
        {
            logger.LogWarning("SmartMeter with id {SmartMeterId} does not belong to user {UserId}",
                policyCreateDto.SmartMeterId, validatedUserId.Id);
            throw new SmartMeterOwnershipException(smartMeter.Id.Id, validatedUserId.Id);
        }
        
        // TODO: Check if location resolution matches with smart meter location

        var policyId = policyRepository.NextIdentity();
        var policy = Policy.Create(policyId, policyCreateDto.MeasurementResolution, policyCreateDto.LocationResolution,
            policyCreateDto.Price, validatedUserId, smartMeter.Id);

        await policyRepository.AddAsync(policy);

        return policyId.Id;
    }
}