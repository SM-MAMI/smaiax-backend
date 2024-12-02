using Microsoft.Extensions.Logging;

using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Exceptions;
using SMAIAXBackend.Application.Services.Interfaces;
using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.Enums;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Domain.Repositories;

namespace SMAIAXBackend.Application.Services.Implementations;

public class PolicyCreateService(
    IPolicyRepository policyRepository,
    ISmartMeterRepository smartMeterRepository,
    ILogger<PolicyCreateService> logger)
    : IPolicyCreateService
{
    public async Task<Guid> CreatePolicyAsync(PolicyCreateDto policyCreateDto)
    {
        var smartMeter = await smartMeterRepository.GetSmartMeterByIdAsync(new SmartMeterId(policyCreateDto.SmartMeterId));

        if (smartMeter == null)
        {
            logger.LogError("Smart meter with id '{SmartMeterId} not found.", policyCreateDto.SmartMeterId);
            throw new SmartMeterNotFoundException(policyCreateDto.SmartMeterId);
        }

        var latestMetadata = smartMeter.Metadata.OrderByDescending(m => m.ValidFrom).FirstOrDefault();

        if (!IsLocationValidForResolution(latestMetadata, policyCreateDto.LocationResolution))
        {
            logger.LogWarning(
                "SmartMeter with id {SmartMeterId} does not have sufficient location data for resolution {LocationResolution}",
                smartMeter.Id.Id, policyCreateDto.LocationResolution);
            throw new InsufficientLocationDataException(smartMeter.Id.Id, policyCreateDto.LocationResolution);
        }

        var policyId = policyRepository.NextIdentity();
        var policy = Policy.Create(policyId, policyCreateDto.Name, policyCreateDto.MeasurementResolution, policyCreateDto.LocationResolution,
            policyCreateDto.Price, smartMeter.Id);

        await policyRepository.AddAsync(policy);

        return policyId.Id;
    }

    /// <summary>
    /// Checks if the location data of the smart meter is sufficient for the given resolution.
    /// If there is no metadata for the smart meter, the location resolution needs to be None.
    /// If there is metadata, the location needs to be as detailed as the resolution.
    /// </summary>
    /// <param name="metadata">The metadata that contains the location that needs to be validated.</param>
    /// <param name="resolution">The resolution specified in the policy.</param>
    /// <returns>True if the location is valid for the resolution, otherwise false.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the resolution is not recognized.</exception>
    private static bool IsLocationValidForResolution(Metadata? metadata, LocationResolution resolution)
    {
        if (metadata == null)
        {
            return resolution == LocationResolution.None;
        }

        return resolution switch
        {
            LocationResolution.StreetName => metadata.Location is
            { StreetName: not null, City: not null, State: not null, Country: not null, Continent: not null },
            LocationResolution.City => metadata.Location is
            { City: not null, State: not null, Country: not null, Continent: not null },
            LocationResolution.State => metadata.Location is { State: not null, Country: not null, Continent: not null },
            LocationResolution.Country => metadata.Location is { Country: not null, Continent: not null },
            LocationResolution.Continent => metadata.Location.Continent != null,
            LocationResolution.None => true,
            _ => throw new ArgumentOutOfRangeException(nameof(resolution), resolution, null)
        };
    }
}