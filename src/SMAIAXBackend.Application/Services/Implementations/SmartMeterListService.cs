using Microsoft.Extensions.Logging;

using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Exceptions;
using SMAIAXBackend.Application.Services.Interfaces;
using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Domain.Repositories;

namespace SMAIAXBackend.Application.Services.Implementations;

public class SmartMeterListService(
    ISmartMeterRepository smartMeterRepository,
    IPolicyRepository policyRepository,
    IUserValidationService userValidationService,
    ILogger<SmartMeterListService> logger) : ISmartMeterListService
{
    public async Task<List<SmartMeterOverviewDto>> GetSmartMetersByUserIdAsync(string? userId)
    {
        var validatedUserId = await userValidationService.ValidateUserAsync(userId);

        List<SmartMeter> smartMeters = await smartMeterRepository.GetSmartMetersByUserIdAsync(validatedUserId);
        var smartMeterOverviewDtos = new List<SmartMeterOverviewDto>();

        foreach (var smartMeter in smartMeters)
        {
            var smartMeterOverviewDto = await SmartMeterOverviewDtoFromSmartMeter(smartMeter, validatedUserId);
            smartMeterOverviewDtos.Add(smartMeterOverviewDto);
        }

        return smartMeterOverviewDtos;
    }

    public async Task<SmartMeterOverviewDto> GetSmartMeterByIdAndUserIdAsync(Guid smartMeterId, string? userId)
    {
        var validatedUserId = await userValidationService.ValidateUserAsync(userId);
        var smartMeter =
            await smartMeterRepository.GetSmartMeterByIdAndUserIdAsync(new SmartMeterId(smartMeterId), validatedUserId);

        if (smartMeter == null)
        {
            logger.LogError("Smart meter with id '{SmartMeterId} not found for user with id '{UserId}'.", smartMeterId,
                validatedUserId.Id);
            throw new SmartMeterNotFoundException(smartMeterId, validatedUserId.Id);
        }

        var smartMeterOverviewDto = await SmartMeterOverviewDtoFromSmartMeter(smartMeter, validatedUserId);

        return smartMeterOverviewDto;
    }

    private async Task<SmartMeterOverviewDto> SmartMeterOverviewDtoFromSmartMeter(SmartMeter smartMeter, UserId userId)
    {
        var policies = await policyRepository.GetPoliciesBySmartMeterIdAndUserIdAsync(smartMeter.Id, userId);
        return new SmartMeterOverviewDto(smartMeter.Id.Id, smartMeter.Name,
            smartMeter.Metadata.Count, policies.Count);
    }
}