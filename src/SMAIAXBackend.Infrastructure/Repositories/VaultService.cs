using Microsoft.Extensions.Options;

using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Domain.Repositories;
using SMAIAXBackend.Infrastructure.Configurations;

using VaultSharp;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.SecretsEngines.ActiveDirectory.Models;
using VaultSharp.V1.SecretsEngines.Database;
using VaultSharp.V1.SecretsEngines.Database.Models.PostgreSQL;

namespace SMAIAXBackend.Infrastructure.Repositories;

public class VaultService : IVaultService
{
    private readonly IVaultClient _vaultClient;
    private readonly string _databaseHost;
    private readonly int _databasePort;
    private readonly string _databaseSuperUsername;
    private readonly string _databaseSuperUserPassword;
    private readonly string _credentialsDefaultTimeToLive;
    private readonly string _credentialsMaximumTimeToLive;
    private const string KeyValueSecretsMountPoint = "secret";

    public VaultService(
        IOptions<VaultConfiguration> vaultConfigOptions,
        IOptions<DatabaseConfiguration> databaseConfigOptions)
    {
        var authMethod = new TokenAuthMethodInfo(vaultConfigOptions.Value.Token);
        _vaultClient = new VaultClient(new VaultClientSettings(vaultConfigOptions.Value.Address, authMethod));
        _databaseHost = vaultConfigOptions.Value.DatabaseHost;
        _databasePort = vaultConfigOptions.Value.DatabasePort;
        _credentialsDefaultTimeToLive = vaultConfigOptions.Value.CredentialsDefaultTimeToLive;
        _credentialsMaximumTimeToLive = vaultConfigOptions.Value.CredentialsMaximumTimeToLive;
        _databaseSuperUsername = databaseConfigOptions.Value.SuperUsername;
        _databaseSuperUserPassword = databaseConfigOptions.Value.SuperUserPassword;
    }

    public async Task CreateDatabaseRoleAsync(string roleName, string databaseName)
    {
        var databaseConnectionConfig = new PostgreSQLConnectionConfigModel
        {
            UsernameTemplate = "",
            AllowedRoles = [roleName],
            ConnectionUrl =
                $"postgresql://{{{{username}}}}:{{{{password}}}}@{_databaseHost}:{_databasePort}/{databaseName}",
            Username = _databaseSuperUsername,
            Password = _databaseSuperUserPassword
        };

        await _vaultClient.V1.Secrets.Database.ConfigureConnectionAsync(databaseName, databaseConnectionConfig);

        var role = new Role
        {
            DatabaseProviderType = new DatabaseProviderType(databaseName),
            DefaultTimeToLive = _credentialsDefaultTimeToLive,
            MaximumTimeToLive = _credentialsMaximumTimeToLive,
            CreationStatements =
            [
                "CREATE ROLE \"{{name}}\" WITH LOGIN PASSWORD '{{password}}' VALID UNTIL '{{expiration}}';",
                $"GRANT CONNECT ON DATABASE {databaseName} TO \"{{{{name}}}}\";",
                "GRANT USAGE ON SCHEMA domain TO \"{{name}}\";",
                "GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA domain TO \"{{name}}\";",
                "ALTER DEFAULT PRIVILEGES IN SCHEMA domain GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO \"{{name}}\";",
                "REVOKE ALL ON SCHEMA public FROM \"{{name}}\";"
            ]
        };

        await _vaultClient.V1.Secrets.Database.CreateRoleAsync(roleName, role);
    }

    public async Task<(string Username, string Password)> GetDatabaseCredentialsAsync(string roleName)
    {
        try
        {
            var secrets = await _vaultClient.V1.Secrets.Database.GetCredentialsAsync(roleName);
            return (secrets.Data.Username, secrets.Data.Password);
        }
        catch (Exception)
        {
            // In case vault is not available return the superuser credentials
            return (_databaseSuperUsername, _databaseSuperUserPassword);
        }
    }

    public async Task SaveMqttBrokerCredentialsAsync(SmartMeterId smartMeterId, string topic, string username, string password)
    {
        var path = $"mqtt/{smartMeterId.Id}";
        var secrets = new Dictionary<string, object>
        {
            ["topic"] = topic,
            ["username"] = username,
            ["password"] = password
        };
        
        await _vaultClient.V1.Secrets.KeyValue.V2.WriteSecretAsync(path, secrets, mountPoint: KeyValueSecretsMountPoint);
    }
}