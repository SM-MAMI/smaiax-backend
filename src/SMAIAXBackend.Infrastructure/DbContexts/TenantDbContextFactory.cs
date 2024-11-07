using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using Npgsql;

using SMAIAXBackend.Infrastructure.Configurations;

namespace SMAIAXBackend.Infrastructure.DbContexts;

public class TenantDbContextFactory(IOptions<DatabaseConfiguration> databaseConfigOptions) : ITenantDbContextFactory
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
}