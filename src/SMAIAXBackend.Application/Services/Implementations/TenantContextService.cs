using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using SMAIAXBackend.Application.Exceptions;
using SMAIAXBackend.Application.Services.Interfaces;
using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Domain.Repositories;

namespace SMAIAXBackend.Application.Services.Implementations;

public class TenantContextService(
    ITenantRepository tenantRepository,
    IUserRepository userRepository,
    IHttpContextAccessor httpContextAccessor,
    ILogger<TenantContextService> logger) : ITenantContextService
{
    public async Task<Tenant> GetCurrentTenantAsync()
    {
        var userId = httpContextAccessor.HttpContext?.Items["UserId"]?.ToString();
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

        var user = await userRepository.GetUserByIdAsync(new UserId(userIdGuid));

        if (user is null)
        {
            logger.LogWarning("User with id {UserId} not found in database.", userIdGuid);
            throw new UserNotFoundException(userIdGuid);
        }

        var tenant = await tenantRepository.GetByIdAsync(user.TenantId);

        if (tenant == null)
        {
            logger.LogWarning("Tenant with id '{TenantId}' not found for user with id '{UserId}'.", user.TenantId.Id,
                user.Id.Id);
            throw new TenantNotFoundException(user.TenantId.Id, user.Id.Id);
        }

        return tenant;
    }
}