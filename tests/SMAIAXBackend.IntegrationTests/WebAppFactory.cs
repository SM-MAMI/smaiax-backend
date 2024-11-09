using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace SMAIAXBackend.IntegrationTests;

public class WebAppFactory(int postgresMappedPublicPort) : WebApplicationFactory<Program>
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
        ["JwtConfiguration:RefreshTokenExpirationMinutes"] = "10080"
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