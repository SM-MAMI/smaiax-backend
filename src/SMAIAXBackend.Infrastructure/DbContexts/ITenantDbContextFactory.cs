namespace SMAIAXBackend.Infrastructure.DbContexts;

public interface ITenantDbContextFactory
{
    TenantDbContext CreateDbContext(string databaseName, string databaseUserName, string databasePassword);
}