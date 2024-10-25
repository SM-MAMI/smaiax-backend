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
    IUserRepository userRepository,
    ILogger<SmartMeterListService> logger) : ISmartMeterListService
{
    public async Task<List<SmartMeterOverviewDto>> GetSmartMetersByUserIdAsync(string? userId)
    {
        // TODO: To not always duplicate the code we could move the userId check code to a helper class
        if (string.IsNullOrEmpty(userId))
        {
            logger.LogWarning("No user claim found in claims principal.");
            throw new InvalidTokenException();
        }

        if (!Guid.TryParse(userId, out var userIdGuid))
        {
            logger.LogWarning("Invalid user claim found in claims principal.");
            throw new InvalidTokenException();
        }

        // TODO: Maybe implement a UserExists method?
        var user = await userRepository.GetUserByIdAsync(new UserId(userIdGuid));

        if (user is null)
        {
            logger.LogWarning("User with id {UserId} not found in database.", userIdGuid);
            throw new UserNotFoundException(userIdGuid);
        }

        List<SmartMeter> smartMeters = await smartMeterRepository.GetSmartMetersByUserIdAsync(new UserId(userIdGuid));
        var smartMeterOverviewDtos = new List<SmartMeterOverviewDto>();

        foreach (var smartMeter in smartMeters)
        {
            var smartMeterOverviewDto = new SmartMeterOverviewDto(smartMeter.Id.Id, smartMeter.Name,
                smartMeter.Metadata.Count, smartMeter.Policies.Count);
            smartMeterOverviewDtos.Add(smartMeterOverviewDto);
        }

        return smartMeterOverviewDtos;
    }
}