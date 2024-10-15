using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.DependencyInjection;
using SMAIAXBackend.Infrastructure.DbContexts;
using Testcontainers.PostgreSql;

namespace SMAIAXBackend.IntegrationTests;

[SetUpFixture]
internal static class IntegrationTestSetup
{
    private static PostgreSqlContainer _postgresContainer = null!;
    private static IContainer _hiveMqContainer = null!;
    private static WebAppFactory _webAppFactory = null!;
    public static ApplicationDbContext ApplicationDbContext { get; private set; } = null!;
    public static HttpClient HttpClient { get; private set; } = null!;

    [OneTimeSetUp]
    public static async Task OneTimeSetup()
    {
        _postgresContainer = new PostgreSqlBuilder()
            .WithImage("postgres:16-bullseye")
            .WithUsername("user")
            .WithPassword("password")
            .WithDatabase("smaiax-db")
            .WithPortBinding(5432, true)
            .Build();

        await _postgresContainer.StartAsync();
        
        _hiveMqContainer = new ContainerBuilder()
            .WithImage("hivemq/hivemq4")
            .WithPortBinding(1883, true)
            .Build();

        await _hiveMqContainer.StartAsync();
        
        var postgresConnectionString = _postgresContainer.GetConnectionString();
        var hiveMqPort = _hiveMqContainer.GetMappedPublicPort(1883);
        _webAppFactory = new WebAppFactory(postgresConnectionString, hiveMqPort);

        HttpClient = _webAppFactory.CreateClient();

        ApplicationDbContext = _webAppFactory.Services.GetRequiredService<ApplicationDbContext>();
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