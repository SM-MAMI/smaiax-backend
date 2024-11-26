using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace SMAIAXBackend.IntegrationTests;

public class WebAppFactory(int postgresMappedPublicPort, int postgresInternalPort, string postgresContainerName, int vaultMappedPublicPort)
    : WebApplicationFactory<Program>
{
    private readonly Dictionary<string, string> _testAppSettings = new()
    {
        ["DatabaseConfiguration:Host"] = "localhost",
        ["DatabaseConfiguration:Port"] = $"{postgresMappedPublicPort}",
        ["DatabaseConfiguration:SuperUsername"] = "user",
        ["DatabaseConfiguration:SuperUserPassword"] = "password",
        ["DatabaseConfiguration:MainDatabase"] = "smaiax-db",
        ["JwtConfiguration:Secret"] = "YourNewStrongSecretKeyOfAtLeast32Characters!",
        ["JwtConfiguration:Issuer"] = "SMAIAX",
        ["JwtConfiguration:Audience"] = "SomeAudience",
        ["JwtConfiguration:AccessTokenExpirationMinutes"] = "60",
        ["JwtConfiguration:RefreshTokenExpirationMinutes"] = "10080",
        ["Vault:Address"] = $"http://localhost:{vaultMappedPublicPort}",
        ["Vault:Token"] = "00000000-0000-0000-0000-000000000000",
        ["Vault:DatabaseHost"] = postgresContainerName,
        ["Vault:DatabasePort"] = $"{postgresInternalPort}"
    };

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        ConfigureAppSettings(builder);
        base.ConfigureWebHost(builder);

        builder.UseEnvironment("Test");
    }

    private void ConfigureAppSettings(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(configBuilder => { configBuilder.AddInMemoryCollection(_testAppSettings!); });
    }
}