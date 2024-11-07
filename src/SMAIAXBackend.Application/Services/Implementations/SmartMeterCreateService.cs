using Microsoft.Extensions.Logging;

using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Exceptions;
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
            logger.LogWarning("Tenant with id '{TenantId}' not found for user with id '{UserId}'.", user.TenantId.Id, user.Id.Id);
            throw new TenantNotFoundException(user.TenantId.Id, user.Id.Id);
        }
        
        var smartMeterId = smartMeterRepository.NextIdentity();
        var smartMeter = SmartMeter.Create(smartMeterId, smartMeterCreateDto.Name, user.Id);
        await smartMeterRepository.AddAsync(smartMeter, tenant);

        return smartMeterId.Id;
    }
}