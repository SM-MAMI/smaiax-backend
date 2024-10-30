using Microsoft.Extensions.Logging;

using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Exceptions;
using SMAIAXBackend.Application.Services.Interfaces;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Domain.Repositories;

namespace SMAIAXBackend.Application.Services.Implementations;

public class SmartMeterUpdateService(
    ISmartMeterRepository smartMeterRepository,
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

        var validatedUserId = await userValidationService.ValidateUserAsync(userId);

        var smartMeter =
            await smartMeterRepository.GetSmartMeterByIdAndUserIdAsync(new SmartMeterId(smartMeterIdExpected),
                validatedUserId);

        if (smartMeter == null)
        {
            logger.LogWarning("SmartMeter not found for id `{SmartMeterId}` and userId `{validatedUserId}`",
                smartMeterIdExpected, validatedUserId);
            throw new SmartMeterNotFoundException(smartMeterIdExpected, validatedUserId.Id);
        }

        smartMeter.Update(smartMeterUpdateDto.Name);
        await smartMeterRepository.UpdateAsync(smartMeter);

        return smartMeterIdExpected;
    }
}