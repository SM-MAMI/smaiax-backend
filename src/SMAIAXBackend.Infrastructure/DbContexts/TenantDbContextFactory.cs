using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using SMAIAXBackend.Infrastructure.Configurations;

namespace SMAIAXBackend.Infrastructure.DbContexts;

public class TenantDbContextFactory(IOptions<DatabaseConfiguration> databaseConfigOptions) : ITenantDbContextFactory
{
    public TenantDbContext CreateDbContext(string databaseName, string databaseUserName, string databasePassword)
    {
        var connectionString =
            $"Host={databaseConfigOptions.Value.Host}:{databaseConfigOptions.Value.Port};Username={databaseUserName};Password={databasePassword};Database={databaseName};";
        var optionsBuilder = new DbContextOptionsBuilder<TenantDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new TenantDbContext(optionsBuilder.Options);
    }
}