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

    public async Task CreateDatabaseForTenantAsync(
        string databaseName,
        string databaseUserName,
        string databasePassword)
    {
        await OpenDatabaseConnectionAsync(applicationDbContext);
        await CreateDatabaseAsync(databaseName);
        await CreateUserAsync(databaseUserName, databasePassword);
        await GrantDatabasePrivilegesAsync(databaseName, databaseUserName);
        await CloseDatabaseConnectionAsync(applicationDbContext);
        
        var tenantDbContext = tenantDbContextFactory.CreateDbContext(databaseName,
            databaseConfigOptions.Value.SuperUsername, databaseConfigOptions.Value.SuperUserPassword);
        await tenantDbContext.Database.MigrateAsync();
        
        await OpenDatabaseConnectionAsync(tenantDbContext);
        await SetSchemaPrivilegesAsync(tenantDbContext, databaseUserName);
        await CloseDatabaseConnectionAsync(tenantDbContext);
    }

    private static async Task OpenDatabaseConnectionAsync(DbContext dbContext)
    {
        await dbContext.Database.OpenConnectionAsync();
    }

    private async Task CreateDatabaseAsync(string databaseName)
    {
        await using var createDbCommand = applicationDbContext.Database.GetDbConnection().CreateCommand();
        createDbCommand.CommandText = $"CREATE DATABASE {databaseName};";
        await createDbCommand.ExecuteNonQueryAsync();
    }

    private async Task CreateUserAsync(string databaseUserName, string databasePassword)
    {
        await using var createUserCommand = applicationDbContext.Database.GetDbConnection().CreateCommand();
        createUserCommand.CommandText = $"CREATE USER {databaseUserName} WITH PASSWORD '{databasePassword}';";
        await createUserCommand.ExecuteNonQueryAsync();
    }

    private async Task GrantDatabasePrivilegesAsync(string databaseName, string databaseUserName)
    {
        await using var grantPrivilegesCommand = applicationDbContext.Database.GetDbConnection().CreateCommand();
        grantPrivilegesCommand.CommandText = $"GRANT CONNECT ON DATABASE {databaseName} TO {databaseUserName};";
        await grantPrivilegesCommand.ExecuteNonQueryAsync();
    }

    private static async Task CloseDatabaseConnectionAsync(DbContext dbContext)
    {
        await dbContext.Database.CloseConnectionAsync();
    }

    private static async Task SetSchemaPrivilegesAsync(DbContext tenantDbContext, string databaseUserName)
    {
        await using var setSchemaPrivilegesCommand = tenantDbContext.Database.GetDbConnection().CreateCommand();

        setSchemaPrivilegesCommand.CommandText = $@"
    GRANT USAGE ON SCHEMA domain TO {databaseUserName};
    GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA domain TO {databaseUserName};
    ";
        await setSchemaPrivilegesCommand.ExecuteNonQueryAsync();

        setSchemaPrivilegesCommand.CommandText = $@"
    ALTER DEFAULT PRIVILEGES IN SCHEMA domain 
    GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO {databaseUserName};
    ";
        await setSchemaPrivilegesCommand.ExecuteNonQueryAsync();

        setSchemaPrivilegesCommand.CommandText = $"REVOKE ALL ON SCHEMA public FROM {databaseUserName};";
        await setSchemaPrivilegesCommand.ExecuteNonQueryAsync();
    }
}