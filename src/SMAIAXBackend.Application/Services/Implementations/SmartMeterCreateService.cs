using Microsoft.Extensions.Logging;

using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Services.Interfaces;
using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Repositories;

namespace SMAIAXBackend.Application.Services.Implementations;

public class SmartMeterCreateService(
    ISmartMeterRepository smartMeterRepository,
    ITenantRepository tenantRepository,
    IUserValidationService userValidationService,
    ILogger<SmartMeterCreateService> logger) : ISmartMeterCreateService
{
    public async Task<Guid> AddSmartMeterAsync(SmartMeterCreateDto smartMeterCreateDto, string? userId)
    {
        var user = await userValidationService.ValidateUserAsync(userId);
        var tenant = await tenantRepository.GetByIdAsync(user.TenantId);

        if (tenant == null)
        {
            // TODO: Throw custom exception
            logger.LogWarning("Tenant not found for user {userId}", userId);
            throw new Exception("Tenant not found");
        }
        
        var smartMeterId = smartMeterRepository.NextIdentity();
        var smartMeter = SmartMeter.Create(smartMeterId, smartMeterCreateDto.Name, user.Id);
        await smartMeterRepository.AddAsync(smartMeter, tenant);

        return smartMeterId.Id;
    }
}