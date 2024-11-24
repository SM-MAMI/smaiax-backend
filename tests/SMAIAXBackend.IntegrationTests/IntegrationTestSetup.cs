using System.Net;

using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;

using Microsoft.Extensions.DependencyInjection;

using SMAIAXBackend.Domain.Repositories;
using SMAIAXBackend.Infrastructure.DbContexts;
using SMAIAXBackend.Infrastructure.Repositories;

using Testcontainers.PostgreSql;

namespace SMAIAXBackend.IntegrationTests;

[SetUpFixture]
internal static class IntegrationTestSetup
{
    private static INetwork _testNetwork = null!;
    private static PostgreSqlContainer _postgresContainer = null!;
    private static IContainer _vaultContainer = null!;
    private static WebAppFactory _webAppFactory = null!;
    public static ApplicationDbContext ApplicationDbContext { get; private set; } = null!;
    public static TenantDbContext TenantDbContext { get; private set; } = null!;
    public static ISmartMeterRepository SmartMeterRepository { get; private set; } = null!;
    public static IPolicyRepository PolicyRepository { get; private set; } = null!;
    public static IPolicyRequestRepository PolicyRequestRepository { get; private set; } = null!;
    public static IUserRepository UserRepository { get; private set; } = null!;
    public static ITenantRepository TenantRepository { get; private set; } = null!;
    public static IVaultService VaultService { get; private set; } = null!;
    public static HttpClient HttpClient { get; private set; } = null!;
    public static string AccessToken { get; private set; } = null!;

    [OneTimeSetUp]
    public static async Task OneTimeSetup()
    {
        const string networkName = "shared-test-network";
        _testNetwork = new NetworkBuilder()
            .WithName(networkName)
            .Build();

        await _testNetwork.CreateAsync();

        const string postgresContainerName = "postgres-test";
        const int postgresPort = 5432;
        const string superUserName = "user";
        const string superUserPassword = "password";
        const string databaseName = "smaiax-db";
        _postgresContainer = new PostgreSqlBuilder()
            .WithImage("postgres:16-bullseye")
            .WithName(postgresContainerName)
            .WithUsername(superUserName)
            .WithPassword(superUserPassword)
            .WithDatabase(databaseName)
            .WithPortBinding(postgresPort, true)
            .WithNetwork(_testNetwork)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(postgresPort))
            .Build();

        await _postgresContainer.StartAsync();

        const int vaultPort = 8200;
        _vaultContainer = new ContainerBuilder()
            .WithImage("vault:1.13.3")
            .WithPortBinding(vaultPort, true)
            .WithEnvironment("VAULT_ADDR", "http://0.0.0.0:8200")
            .WithEnvironment("VAULT_DEV_ROOT_TOKEN_ID", "00000000-0000-0000-0000-000000000000")
            .WithEnvironment("VAULT_TOKEN", "00000000-0000-0000-0000-000000000000")
            .WithBindMount(Path.GetFullPath("../../../../../vault/"), "/vault/")
            .WithCommand("./vault/config/entrypoint.sh")
            .WithNetwork(_testNetwork)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilFileExists("/tmp/healthy", FileSystem.Container))
            .Build();

        await _vaultContainer.StartAsync();

        var postgresMappedPublicPort = _postgresContainer.GetMappedPublicPort(postgresPort);
        var vaultMappedPublicPort = _vaultContainer.GetMappedPublicPort(vaultPort);
        _webAppFactory = new WebAppFactory(postgresMappedPublicPort, postgresPort, postgresContainerName,
            vaultMappedPublicPort);

        HttpClient = _webAppFactory.CreateClient();

        ApplicationDbContext = _webAppFactory.Services.GetRequiredService<ApplicationDbContext>();
        var tenantDbContextFactory = _webAppFactory.Services.GetRequiredService<ITenantDbContextFactory>();
        TenantDbContext = tenantDbContextFactory.CreateDbContext("tenant_1_db", superUserName, superUserPassword);
        TenantRepository = _webAppFactory.Services.GetRequiredService<ITenantRepository>();
        UserRepository = _webAppFactory.Services.GetRequiredService<IUserRepository>();
        VaultService = _webAppFactory.Services.GetRequiredService<IVaultService>();

        // Repositories that are using the TenantDatabase need to be instantiated because
        // They are injected with a connection string that is created based on the http request
        SmartMeterRepository = new SmartMeterRepository(TenantDbContext);
        PolicyRepository = new PolicyRepository(TenantDbContext);
        PolicyRequestRepository = new PolicyRequestRepository(TenantDbContext);

        var tokenRepository = _webAppFactory.Services.GetRequiredService<ITokenRepository>();
        AccessToken = await tokenRepository.GenerateAccessTokenAsync($"{Guid.NewGuid()}-{Guid.NewGuid()}",
            "3c07065a-b964-44a9-9cdf-fbd49d755ea7", "johndoe", "john.doe@example.com");
    }

    [OneTimeTearDown]
    public static async Task OneTimeTearDown()
    {
        await _vaultContainer.StopAsync();
        await _vaultContainer.DisposeAsync();
        await _postgresContainer.StopAsync();
        await _postgresContainer.DisposeAsync();
        await _testNetwork.DeleteAsync();
        await _testNetwork.DisposeAsync();
        HttpClient.Dispose();
        await _webAppFactory.DisposeAsync();
    }
}