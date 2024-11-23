using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.Domain.Repositories;

public interface ITenantRepository
{
    TenantId NextIdentity();
    Task AddAsync(Tenant tenant);
    Task DeleteAsync(Tenant tenant);
    Task<Tenant?> GetByIdAsync(TenantId tenantId);
    Task CreateDatabaseForTenantAsync(string databaseName);
}