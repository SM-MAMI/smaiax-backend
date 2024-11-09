using Microsoft.Extensions.Logging;

using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Exceptions;
using SMAIAXBackend.Application.Services.Interfaces;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Domain.Repositories;

namespace SMAIAXBackend.Application.Services.Implementations;

public class SmartMeterUpdateService(
    ISmartMeterRepository smartMeterRepository,
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

        var smartMeter =
            await smartMeterRepository.GetSmartMeterByIdAsync(new SmartMeterId(smartMeterIdExpected));

        if (smartMeter == null)
        {
            logger.LogError("Smart meter with id '{SmartMeterId} not found.", smartMeterIdExpected);
            throw new SmartMeterNotFoundException(smartMeterIdExpected);
        }

        smartMeter.Update(smartMeterUpdateDto.Name);
        await smartMeterRepository.UpdateAsync(smartMeter);

        return smartMeterIdExpected;
    }
}