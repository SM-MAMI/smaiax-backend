using Microsoft.Extensions.Logging;

using SMAIAXBackend.Application.Exceptions;
using SMAIAXBackend.Application.Services.Interfaces;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Domain.Repositories;

namespace SMAIAXBackend.Application.Services.Implementations;

public class SmartMeterDeleteService(
    ISmartMeterRepository smartMeterRepository,
    ILogger<SmartMeterDeleteService> logger) : ISmartMeterDeleteService
{
    public async Task RemoveMetadataFromSmartMeterAsync(Guid smartMeterId, Guid metadataId)
    {
        var smartMeter =
            await smartMeterRepository.GetSmartMeterByIdAsync(new SmartMeterId(smartMeterId));

        if (smartMeter == null)
        {
            logger.LogError("Smart meter with id '{SmartMeterId} not found.", smartMeterId);
            throw new SmartMeterNotFoundException(smartMeterId);
        }

        smartMeter.RemoveMetadata(new MetadataId(metadataId));
        await smartMeterRepository.UpdateAsync(smartMeter);
    }
}