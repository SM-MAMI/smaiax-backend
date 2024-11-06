using Microsoft.Extensions.Logging;

using SMAIAXBackend.Application.Exceptions;
using SMAIAXBackend.Application.Services.Interfaces;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Domain.Repositories;

namespace SMAIAXBackend.Application.Services.Implementations;

public class SmartMeterDeleteService(
    IUserValidationService userValidationService,
    ISmartMeterRepository smartMeterRepository,
    ILogger<SmartMeterDeleteService> logger) : ISmartMeterDeleteService
{
    public async Task RemoveMetadataFromSmartMeterAsync(Guid smartMeterId, Guid metadataId, string? userId)
    {
        var validatedUserId = await userValidationService.ValidateUserAsync(userId);
        var smartMeter =
            await smartMeterRepository.GetSmartMeterByIdAndUserIdAsync(new SmartMeterId(smartMeterId), validatedUserId);

        if (smartMeter == null)
        {
            logger.LogWarning("SmartMeter with id {SmartMeterId} not found for user {UserId}", smartMeterId,
                validatedUserId.Id);
            throw new SmartMeterNotFoundException(smartMeterId, validatedUserId.Id);
        }

        smartMeter.RemoveMetadata(new MetadataId(metadataId));
        await smartMeterRepository.UpdateAsync(smartMeter);
    }
}