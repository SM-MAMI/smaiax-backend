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
    ITenantContextService tenantContextService,
    ILogger<SmartMeterListService> logger) : ISmartMeterListService
{
    public async Task<List<SmartMeterOverviewDto>> GetSmartMetersAsync()
    {
        var tenant = await tenantContextService.GetCurrentTenantAsync();
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
        var tenant = await tenantContextService.GetCurrentTenantAsync();
        var smartMeter =
            await smartMeterRepository.GetSmartMeterByIdAsync(new SmartMeterId(smartMeterId), tenant);

        if (smartMeter == null)
        {
            logger.LogError("Smart meter with id '{SmartMeterId} not found for tenant with id '{TenantId}'.", smartMeterId,
                tenant.Id);
            throw new SmartMeterNotFoundException(smartMeterId, tenant.Id.Id);
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