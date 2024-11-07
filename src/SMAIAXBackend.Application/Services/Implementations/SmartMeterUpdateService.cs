using Microsoft.Extensions.Logging;

using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Exceptions;
using SMAIAXBackend.Application.Services.Interfaces;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Domain.Repositories;

namespace SMAIAXBackend.Application.Services.Implementations;

public class SmartMeterUpdateService(
    ISmartMeterRepository smartMeterRepository,
    ITenantContextService tenantContextService,
    ILogger<SmartMeterUpdateService> logger) : ISmartMeterUpdateService
{
    public async Task<Guid> UpdateSmartMeterAsync(
        Guid smartMeterIdExpected,
        SmartMeterUpdateDto smartMeterUpdateDto)
    {
        if (smartMeterIdExpected != smartMeterUpdateDto.Id)
        {
            logger.LogWarning(
                "SmartMeterId `{SmartMeterIdExpected}` in the path does not match the SmartMeterId `{SmartMeterIdActual}` in the body",
                smartMeterIdExpected, smartMeterUpdateDto.Id);
            throw new SmartMeterIdMismatchException(smartMeterIdExpected, smartMeterUpdateDto.Id);
        }

        var tenant = await tenantContextService.GetCurrentTenantAsync();
        var smartMeter =
            await smartMeterRepository.GetSmartMeterByIdAsync(new SmartMeterId(smartMeterIdExpected),
                tenant);

        if (smartMeter == null)
        {
            logger.LogError("Smart meter with id '{SmartMeterId} not found for tenant with id '{TenantId}'.", smartMeterIdExpected,
                tenant.Id);
            throw new SmartMeterNotFoundException(smartMeterIdExpected, tenant.Id.Id);
        }

        smartMeter.Update(smartMeterUpdateDto.Name);
        await smartMeterRepository.UpdateAsync(smartMeter, tenant);

        return smartMeterIdExpected;
    }
}