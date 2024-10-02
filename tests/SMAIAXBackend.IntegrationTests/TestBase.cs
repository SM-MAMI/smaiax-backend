using SMAIAXBackend.Infrastructure.DbContexts;

namespace SMAIAXBackend.IntegrationTests;

public class TestBase
{
    protected readonly HttpClient HttpClient = IntegrationTestSetup.HttpClient;
    protected readonly UserStoreDbContext UserStoreDbContext = IntegrationTestSetup.UserStoreDbContext;
    
    [SetUp]
    public async Task Setup()
    {
        await IntegrationTestSetup.UserStoreDbContext.Database.EnsureCreatedAsync();
        IntegrationTestSetup.UserStoreDbContext.ChangeTracker.Clear();
    }

    [TearDown]
    public async Task TearDown()
    {
        await IntegrationTestSetup.UserStoreDbContext.Database.EnsureDeletedAsync();
    }
}