using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using Npgsql;

using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Repositories;
using SMAIAXBackend.Infrastructure.Configurations;

namespace SMAIAXBackend.Infrastructure.DbContexts;

public class TenantDbContextFactory(IOptions<DatabaseConfiguration> databaseConfigOptions, IVaultService vaultService) : ITenantDbContextFactory
{
    public TenantDbContext CreateDbContext(string databaseName, string databaseUserName, string databasePassword)
    {
        var connectionStringBuilder = new NpgsqlConnectionStringBuilder
        {
            Host = databaseConfigOptions.Value.Host,
            Port = databaseConfigOptions.Value.Port,
            Username = databaseUserName,
            Password = databasePassword,
            Database = databaseName
        };
        var optionsBuilder = new DbContextOptionsBuilder<TenantDbContext>();
        optionsBuilder.UseNpgsql(connectionStringBuilder.ConnectionString);

        return new TenantDbContext(optionsBuilder.Options);
    }

    public async Task<string> GetConnectionStringForTenant(Tenant tenant)
    {
        var credentials = await vaultService.GetDatabaseCredentialsAsync(tenant.VaultRoleName);

        var connectionStringBuilder = new NpgsqlConnectionStringBuilder
        {
            Host = databaseConfigOptions.Value.Host,
            Port = databaseConfigOptions.Value.Port,
            Username = credentials.Username,
            Password = credentials.Password,
            Database = tenant.DatabaseName
        };

        return connectionStringBuilder.ConnectionString;
    }
}