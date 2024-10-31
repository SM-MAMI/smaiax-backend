using Microsoft.Extensions.Logging;

using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Exceptions;
using SMAIAXBackend.Application.Services.Interfaces;
using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Domain.Repositories;

namespace SMAIAXBackend.Application.Services.Implementations;

public class SmartMeterCreateService(
    ISmartMeterRepository smartMeterRepository,
    IUserValidationService userValidationService,
    ILogger<SmartMeterCreateService> logger) : ISmartMeterCreateService
{
    public async Task<Guid> AddSmartMeterAsync(SmartMeterCreateDto smartMeterCreateDto, string? userId)
    {
        var validatedUserId = await userValidationService.ValidateUserAsync(userId);
        var smartMeterId = smartMeterRepository.NextIdentity();
        var smartMeter = SmartMeter.Create(smartMeterId, smartMeterCreateDto.Name, validatedUserId);
        await smartMeterRepository.AddAsync(smartMeter);

        return smartMeterId.Id;
    }

    public async Task<Guid> AddMetadataAsync(Guid smartMeterId, MetadataCreateDto metadataCreateDto, string? userId)
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

        var metadataId = smartMeterRepository.NextMetadataIdentity();
        var location = new Location(metadataCreateDto.Location.StreetName, metadataCreateDto.Location.City,
            metadataCreateDto.Location.State, metadataCreateDto.Location.Country, metadataCreateDto.Location.Continent);
        var metadata = Metadata.Create(metadataId, metadataCreateDto.ValidFrom, location, metadataCreateDto.HouseholdSize, smartMeter.Id);
        smartMeter.AddMetadata(metadata);
        
        await smartMeterRepository.UpdateAsync(smartMeter);

        return smartMeterId;
    }
}