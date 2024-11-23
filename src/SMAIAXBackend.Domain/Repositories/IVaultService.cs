namespace SMAIAXBackend.Domain.Repositories;

public interface IVaultService
{
    Task CreateDatabaseRoleAsync(string roleName, string databaseName);
}