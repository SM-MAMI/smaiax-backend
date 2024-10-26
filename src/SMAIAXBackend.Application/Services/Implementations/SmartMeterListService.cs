using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Services.Interfaces;
using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Repositories;

namespace SMAIAXBackend.Application.Services.Implementations;

public class SmartMeterListService(
    ISmartMeterRepository smartMeterRepository,
    IUserValidationService userValidationService) : ISmartMeterListService
{
    public async Task<List<SmartMeterOverviewDto>> GetSmartMetersByUserIdAsync(string? userId)
    {
        var validatedUserId = await userValidationService.ValidateUserAsync(userId);
        
        List<SmartMeter> smartMeters = await smartMeterRepository.GetSmartMetersByUserIdAsync(validatedUserId);
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