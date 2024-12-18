using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.Domain.Repositories;

public interface IVaultRepository
{
    Task CreateDatabaseRoleAsync(string roleName, string databaseName);
    Task<(string Username, string Password)> GetDatabaseCredentialsAsync(string roleName);
    Task SaveMqttBrokerCredentialsAsync(SmartMeterId smartMeterId, string topic, string username, string password);
    Task<(string? Username, string? Password, string? Topic)> GetMqttBrokerCredentialsAsync(SmartMeterId smartMeterId);
}