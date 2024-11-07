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
    ILogger<SmartMeterListService> logger) : ISmartMeterListService
{
    public async Task<List<SmartMeterOverviewDto>> GetSmartMetersAsync()
    {
        List<SmartMeter> smartMeters = await smartMeterRepository.GetSmartMetersAsync();
        var smartMeterOverviewDtos = new List<SmartMeterOverviewDto>();

        foreach (var smartMeter in smartMeters)
        {
            var smartMeterOverviewDto = SmartMeterOverviewDtoFromSmartMeter(smartMeter);
            smartMeterOverviewDtos.Add(smartMeterOverviewDto);
        }

        return smartMeterOverviewDtos;
    }

    public async Task<SmartMeterOverviewDto> GetSmartMeterByIdAsync(Guid smartMeterId)
    {
        var smartMeter =
            await smartMeterRepository.GetSmartMeterByIdAsync(new SmartMeterId(smartMeterId));

        if (smartMeter == null)
        {
            logger.LogError("Smart meter with id '{SmartMeterId} not found.", smartMeterId);
            throw new SmartMeterNotFoundException(smartMeterId);
        }

        var smartMeterOverviewDto = SmartMeterOverviewDtoFromSmartMeter(smartMeter);

        return smartMeterOverviewDto;
    }

    private static SmartMeterOverviewDto SmartMeterOverviewDtoFromSmartMeter(SmartMeter smartMeter)
    {
        return new SmartMeterOverviewDto(smartMeter.Id.Id, smartMeter.Name,
            smartMeter.Metadata.Count, smartMeter.Policies.Count);
    }
}