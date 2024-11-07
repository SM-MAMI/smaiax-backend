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
    ILogger<SmartMeterCreateService> logger) : ISmartMeterCreateService
{
    public async Task<Guid> AddSmartMeterAsync(SmartMeterCreateDto smartMeterCreateDto)
    {
        var smartMeterId = smartMeterRepository.NextIdentity();
        var smartMeter = SmartMeter.Create(smartMeterId, smartMeterCreateDto.Name);
        await smartMeterRepository.AddAsync(smartMeter);

        return smartMeterId.Id;
    }
    
    public async Task<Guid> AddMetadataAsync(Guid smartMeterId, MetadataCreateDto metadataCreateDto)
    {
        var smartMeter =
            await smartMeterRepository.GetSmartMeterByIdAsync(new SmartMeterId(smartMeterId));

        if (smartMeter == null)
        {
            logger.LogError("Smart meter with id '{SmartMeterId} not found.", smartMeterId);
            throw new SmartMeterNotFoundException(smartMeterId);
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