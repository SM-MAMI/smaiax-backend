using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace SMAIAXBackend.IntegrationTests;

public class WebAppFactory(string postgresConnectionString) : WebApplicationFactory<Program>
{
    private readonly Dictionary<string, string> _testAppSettings = new()
    {
        ["ConnectionStrings:smaiax-db"] = postgresConnectionString,
        ["JwtConfiguration:Secret"] = "YourNewStrongSecretKeyOfAtLeast32Characters!",
        ["JwtConfiguration:Issuer"] = "SMAIAX",
        ["JwtConfiguration:Audience"] = "SomeAudience",
        ["JwtConfiguration:ExpirationMinutes"] = "60"
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