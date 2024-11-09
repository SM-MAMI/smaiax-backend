using SMAIAXBackend.Domain.Model.Entities;

namespace SMAIAXBackend.Application.Services.Interfaces;

public interface ITenantContextService
{
    Task<Tenant> GetCurrentTenantAsync();
}