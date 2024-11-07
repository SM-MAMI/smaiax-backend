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

    public async Task CreateDatabaseForTenantAsync(string databaseName, string databaseUserName, string databasePassword)
    {
        await applicationDbContext.Database.OpenConnectionAsync();
        
        await using var createDbCommand = applicationDbContext.Database.GetDbConnection().CreateCommand();
        createDbCommand.CommandText = $"CREATE DATABASE {databaseName};";
        await createDbCommand.ExecuteNonQueryAsync();

        await using var createUserCommand = applicationDbContext.Database.GetDbConnection().CreateCommand();
        createDbCommand.CommandText = $"CREATE USER {databaseUserName} WITH PASSWORD '{databasePassword}';";
        await createDbCommand.ExecuteNonQueryAsync();
        
        await using var grantPrivilegesCommand = applicationDbContext.Database.GetDbConnection().CreateCommand();
        createDbCommand.CommandText = $"GRANT CONNECT ON DATABASE {databaseName} TO {databaseUserName};";
        await createDbCommand.ExecuteNonQueryAsync();

        await applicationDbContext.Database.CloseConnectionAsync();
        
        var superUsername = databaseConfigOptions.Value.SuperUsername;
        var superPassword = databaseConfigOptions.Value.SuperUserPassword;
        
        if (string.IsNullOrEmpty(superUsername) || string.IsNullOrEmpty(superPassword))
        {
            // TODO: Throw better exception
            throw new ApplicationException("Super username or password is not set in the configuration.");
        }
        
        var tenantDbContext = tenantDbContextFactory.CreateDbContext(databaseName, superUsername, superPassword);
        
        // Creates also the schema 'domain'
        await tenantDbContext.Database.MigrateAsync();
        
        await tenantDbContext.Database.OpenConnectionAsync();
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
        
        await tenantDbContext.Database.CloseConnectionAsync();
    }
}