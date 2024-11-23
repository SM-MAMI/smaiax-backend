using Microsoft.Extensions.Options;

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
    private readonly string _databaseUsername;
    private readonly string _databasePassword;

    public VaultService(IOptions<VaultConfiguration> vaultConfigOptions, IOptions<DatabaseConfiguration> databaseConfigOptions)
    {
        var authMethod = new TokenAuthMethodInfo(vaultConfigOptions.Value.Token);
        _vaultClient = new VaultClient(new VaultClientSettings(vaultConfigOptions.Value.Address, authMethod));
        _databaseHost = vaultConfigOptions.Value.DatabaseHost;
        _databasePort = vaultConfigOptions.Value.DatabasePort;
        _databaseUsername = databaseConfigOptions.Value.SuperUsername;
        _databasePassword = databaseConfigOptions.Value.SuperUserPassword;
    }
    
    public async Task CreateDatabaseRoleAsync(string roleName, string databaseName)
    {
        var databaseConnectionConfig = new PostgreSQLConnectionConfigModel
        {
            UsernameTemplate = "",
            AllowedRoles = [ roleName ],
            ConnectionUrl = $"postgresql://{{{{username}}}}:{{{{password}}}}@{_databaseHost}:{_databasePort}/{databaseName}",
            Username = _databaseUsername,
            Password = _databasePassword
        };
        
        await _vaultClient.V1.Secrets.Database.ConfigureConnectionAsync(databaseName, databaseConnectionConfig);
        
        var role = new Role
        {
            DatabaseProviderType = new DatabaseProviderType(databaseName),
            DefaultTimeToLive = "1h",
            MaximumTimeToLive = "24h",
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
}