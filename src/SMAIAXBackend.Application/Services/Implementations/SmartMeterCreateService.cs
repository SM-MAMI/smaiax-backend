using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Services.Interfaces;
using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Repositories;

namespace SMAIAXBackend.Application.Services.Implementations;

public class SmartMeterCreateService(
    ISmartMeterRepository smartMeterRepository,
    IUserValidationService userValidationService) : ISmartMeterCreateService
{
    public async Task<Guid> AddSmartMeterAsync(SmartMeterCreateDto smartMeterCreateDto, string? userId)
    {
        var validatedUserId = await userValidationService.ValidateUserAsync(userId);
        var smartMeterId = smartMeterRepository.NextIdentity();
        var smartMeter = SmartMeter.Create(smartMeterId, smartMeterCreateDto.Name, validatedUserId);
        await smartMeterRepository.AddAsync(smartMeter);

        return smartMeterId.Id;
    }
}