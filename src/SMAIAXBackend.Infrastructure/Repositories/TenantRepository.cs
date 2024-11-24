using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Domain.Repositories;
using SMAIAXBackend.Infrastructure.Configurations;
using SMAIAXBackend.Infrastructure.DbContexts;

namespace SMAIAXBackend.Infrastructure.Repositories;

public class TenantRepository(
    ApplicationDbContext applicationDbContext,
    ITenantDbContextFactory tenantDbContextFactory,
    IOptions<DatabaseConfiguration> databaseConfigOptions) : ITenantRepository
{
    public TenantId NextIdentity()
    {
        return new TenantId(Guid.NewGuid());
    }

    public async Task AddAsync(Tenant tenant)
    {
        await applicationDbContext.Tenants.AddAsync(tenant);
        await applicationDbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Tenant tenant)
    {
        applicationDbContext.Tenants.Remove(tenant);
        await applicationDbContext.SaveChangesAsync();
    }

    public async Task<Tenant?> GetByIdAsync(TenantId tenantId)
    {
        return await applicationDbContext.Tenants.FindAsync(tenantId);
    }

    public async Task CreateDatabaseForTenantAsync(string databaseName)
    {
        await applicationDbContext.Database.OpenConnectionAsync();

        await using var createDbCommand = applicationDbContext.Database.GetDbConnection().CreateCommand();
        createDbCommand.CommandText = $"CREATE DATABASE {databaseName};";
        await createDbCommand.ExecuteNonQueryAsync();

        await applicationDbContext.Database.CloseConnectionAsync();

        var tenantDbContext = tenantDbContextFactory.CreateDbContext(databaseName,
            databaseConfigOptions.Value.SuperUsername, databaseConfigOptions.Value.SuperUserPassword);
        await tenantDbContext.Database.MigrateAsync();
    }
}