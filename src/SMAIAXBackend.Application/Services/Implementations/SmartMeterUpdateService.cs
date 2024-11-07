using Microsoft.Extensions.Logging;

using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Exceptions;
using SMAIAXBackend.Application.Services.Interfaces;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Domain.Repositories;

namespace SMAIAXBackend.Application.Services.Implementations;

public class SmartMeterUpdateService(
    ISmartMeterRepository smartMeterRepository,
    ITenantRepository tenantRepository,
    IUserValidationService userValidationService,
    ILogger<SmartMeterUpdateService> logger) : ISmartMeterUpdateService
{
    public async Task<Guid> UpdateSmartMeterAsync(
        Guid smartMeterIdExpected,
        SmartMeterUpdateDto smartMeterUpdateDto,
        string? userId)
    {
        if (smartMeterIdExpected != smartMeterUpdateDto.Id)
        {
            logger.LogWarning(
                "SmartMeterId `{SmartMeterIdExpected}` in the path does not match the SmartMeterId `{SmartMeterIdActual}` in the body",
                smartMeterIdExpected, smartMeterUpdateDto.Id);
            throw new SmartMeterIdMismatchException(smartMeterIdExpected, smartMeterUpdateDto.Id);
        }

        var user = await userValidationService.ValidateUserAsync(userId);
        var tenant = await tenantRepository.GetByIdAsync(user.TenantId);

        if (tenant == null)
        {
            // TODO: Throw custom exception
            logger.LogWarning("Tenant not found for user {userId}", userId);
            throw new Exception("Tenant not found");
        }

        var smartMeter =
            await smartMeterRepository.GetSmartMeterByIdAsync(new SmartMeterId(smartMeterIdExpected),
                tenant);

        if (smartMeter == null)
        {
            logger.LogWarning("SmartMeter not found for id `{SmartMeterId}` and userId `{validatedUserId}`",
                smartMeterIdExpected, user.Id);
            throw new SmartMeterNotFoundException(smartMeterIdExpected, user.Id.Id);
        }

        smartMeter.Update(smartMeterUpdateDto.Name);
        await smartMeterRepository.UpdateAsync(smartMeter, tenant);

        return smartMeterIdExpected;
    }
}