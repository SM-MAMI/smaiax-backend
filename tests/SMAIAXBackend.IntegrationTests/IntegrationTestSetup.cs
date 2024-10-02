using Microsoft.Extensions.DependencyInjection;
using SMAIAXBackend.Infrastructure.DbContexts;
using Testcontainers.PostgreSql;

namespace SMAIAXBackend.IntegrationTests;


[SetUpFixture]
internal static class IntegrationTestSetup
{
    private static PostgreSqlContainer _postgresContainer = null!;
    private static WebAppFactory _webAppFactory = null!;
    public static UserStoreDbContext UserStoreDbContext { get; private set; } = null!;
    public static HttpClient HttpClient { get; private set; } = null!;
    
    [OneTimeSetUp]
    public static async Task OneTimeSetup()
    {
        _postgresContainer = new PostgreSqlBuilder()
            .WithImage("postgres:16-bullseye")
            .WithUsername("user")
            .WithPassword("password")
            .WithDatabase("user-store")
            .WithPortBinding(5432, true)
            .Build();

        await _postgresContainer.StartAsync();

        _webAppFactory = new WebAppFactory(_postgresContainer.GetConnectionString());

        HttpClient = _webAppFactory.CreateClient();

        UserStoreDbContext = _webAppFactory.Services.GetRequiredService<UserStoreDbContext>();
    }

    [OneTimeTearDown]
    public static async Task OneTimeTearDown()
    {
        await _postgresContainer.StopAsync();
        await _postgresContainer.DisposeAsync();
        HttpClient.Dispose();
        await _webAppFactory.DisposeAsync();
    }
}