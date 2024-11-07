using Microsoft.AspNetCore.Http;
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
    ITenantRepository tenantRepository,
    IUserValidationService userValidationService,
    IHttpContextAccessor httpContextAccessor,
    ILogger<SmartMeterListService> logger) : ISmartMeterListService
{
    public async Task<List<SmartMeterOverviewDto>> GetSmartMetersAsync()
    {
        var userId = httpContextAccessor.HttpContext?.Items["UserId"]?.ToString();
        var user = await userValidationService.ValidateUserAsync(userId);
        var tenant = await tenantRepository.GetByIdAsync(user.TenantId);

        if (tenant == null)
        {
            logger.LogWarning("Tenant with id '{TenantId}' not found for user with id '{UserId}'.", user.TenantId.Id, user.Id.Id);
            throw new TenantNotFoundException(user.TenantId.Id, user.Id.Id);
        }

        List<SmartMeter> smartMeters = await smartMeterRepository.GetSmartMetersAsync(tenant);
        var smartMeterOverviewDtos = new List<SmartMeterOverviewDto>();

        foreach (var smartMeter in smartMeters)
        {
            var smartMeterOverviewDto = SmartMeterOverviewDtoFromSmartMeter(smartMeter);
            smartMeterOverviewDtos.Add(smartMeterOverviewDto);
        }

        return smartMeterOverviewDtos;
    }

    public async Task<SmartMeterOverviewDto> GetSmartMeterByIdAsync(Guid smartMeterId)
    {
        var userId = httpContextAccessor.HttpContext?.Items["UserId"]?.ToString();
        var user = await userValidationService.ValidateUserAsync(userId);
        var tenant = await tenantRepository.GetByIdAsync(user.TenantId);

        if (tenant == null)
        {
            logger.LogWarning("Tenant with id '{TenantId}' not found for user with id '{UserId}'.", user.TenantId.Id, user.Id.Id);
            throw new TenantNotFoundException(user.TenantId.Id, user.Id.Id);
        }
        var smartMeter =
            await smartMeterRepository.GetSmartMeterByIdAsync(new SmartMeterId(smartMeterId), tenant);

        if (smartMeter == null)
        {
            logger.LogError("Smart meter with id '{SmartMeterId} not found for user with id '{UserId}'.", smartMeterId,
                user.Id);
            throw new SmartMeterNotFoundException(smartMeterId, user.Id.Id);
        }

        var smartMeterOverviewDto = SmartMeterOverviewDtoFromSmartMeter(smartMeter);

        return smartMeterOverviewDto;
    }

    private static SmartMeterOverviewDto SmartMeterOverviewDtoFromSmartMeter(SmartMeter smartMeter)
    {
        return new SmartMeterOverviewDto(smartMeter.Id.Id, smartMeter.Name,
            smartMeter.Metadata.Count, smartMeter.Policies.Count);
    }
}