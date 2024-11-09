using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.Enums;
using SMAIAXBackend.Domain.Model.ValueObjects;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Domain.Repositories;
using SMAIAXBackend.Infrastructure.DbContexts;

namespace SMAIAXBackend.IntegrationTests;

public class TestBase
{
    protected readonly HttpClient _httpClient = IntegrationTestSetup.HttpClient;
    protected readonly ApplicationDbContext _applicationDbContext = IntegrationTestSetup.ApplicationDbContext;
    protected readonly TenantDbContext _tenantDbContext = IntegrationTestSetup.TenantDbContext;
    protected readonly ISmartMeterRepository _smartMeterRepository = IntegrationTestSetup.SmartMeterRepository;
    protected readonly IPolicyRepository _policyRepository = IntegrationTestSetup.PolicyRepository;
    protected readonly IPolicyRequestRepository _policyRequestRepository = IntegrationTestSetup.PolicyRequestRepository;
    protected readonly IUserRepository _userRepository = IntegrationTestSetup.UserRepository;
    protected readonly string _accessToken = IntegrationTestSetup.AccessToken;

    [SetUp]
    public async Task Setup()
    {
        await IntegrationTestSetup.ApplicationDbContext.Database.EnsureCreatedAsync();
        await IntegrationTestSetup.TenantRepository.CreateDatabaseForTenantAsync("tenant_1_db", "johndoe", "P@ssw0rd");
        await InsertTestData();
        IntegrationTestSetup.ApplicationDbContext.ChangeTracker.Clear();
        IntegrationTestSetup.TenantDbContext.ChangeTracker.Clear();
    }

    [TearDown]
    public async Task TearDown()
    {
        IntegrationTestSetup.ApplicationDbContext.ChangeTracker.Clear();
        IntegrationTestSetup.TenantDbContext.ChangeTracker.Clear();
        await IntegrationTestSetup.TenantDbContext.Database.EnsureDeletedAsync();
        await CleanupTenantDatabase();
        await IntegrationTestSetup.ApplicationDbContext.Database.EnsureDeletedAsync();
    }

    private static async Task CleanupTenantDatabase()
    {
        await using var deleteUserCommand = IntegrationTestSetup.ApplicationDbContext.Database.GetDbConnection().CreateCommand();
        deleteUserCommand.CommandText = "DROP ROLE IF EXISTS johndoe;";
        await IntegrationTestSetup.ApplicationDbContext.Database.OpenConnectionAsync();
        await deleteUserCommand.ExecuteNonQueryAsync();
        await IntegrationTestSetup.ApplicationDbContext.Database.CloseConnectionAsync();
    }

    private async Task InsertTestData()
    {
        var hasher = new PasswordHasher<IdentityUser>();

        var userId = new UserId(Guid.Parse("3c07065a-b964-44a9-9cdf-fbd49d755ea7"));
        const string userName = "johndoe";
        const string email = "john.doe@example.com";
        const string password = "P@ssw0rd";
        var testUser = new IdentityUser
        {
            Id = userId.Id.ToString(),
            UserName = userName,
            NormalizedUserName = userName.ToUpper(),
            Email = email,
            NormalizedEmail = email.ToUpper(),
        };
        var passwordHash = hasher.HashPassword(testUser, password);
        testUser.PasswordHash = passwordHash;

        var tenantId = new TenantId(Guid.Parse("f4c70232-6715-4c15-966f-bf4bcef46d39"));
        var tenant = Tenant.Create(tenantId, userName, password, "tenant_1_db");
        var domainUser = User.Create(userId, new Name("John", "Doe"), userName, email, tenantId);

        // Valid refresh token
        const string jwtId = "19f77b2e-e485-4031-8506-62f6d3b69e4d";
        const string token1 = "4dffb63c-581d-4588-8b4b-4b075f17d015-abcb30f4-5f32-4fbb-80c4-99cea98273ca";
        var expirationDate1 = DateTime.UtcNow.AddDays(100);
        var refreshToken1 = RefreshToken.Create(
            new RefreshTokenId(Guid.Parse("21938ead-d43f-4f16-a055-c3b5613cd599")),
            userId,
            jwtId,
            token1,
            true,
            expirationDate1
        );

        // Invalid refresh token
        const string token2 = "266cbbdb-edcd-48a6-aa63-f837b05a2551-3b01aaa3-304a-434b-bc7d-fd9a6305550b";
        var refreshToken2 = RefreshToken.Create(
            new RefreshTokenId(Guid.Parse("ad758462-20de-41fc-91d4-0569466224fc")),
            userId,
            jwtId,
            token2,
            false,
            expirationDate1
        );

        // Expired refresh token
        const string token3 = "01318f82-8307-480d-bbb6-f3be92ba7480-b903a69c-7d76-4ce1-8dce-d325712bf240";
        var expirationDate2 = DateTime.UtcNow.AddDays(-10);
        var refreshToken3 = RefreshToken.Create(
            new RefreshTokenId(Guid.Parse("9674b31b-eee3-47c1-be45-f49c4c3004f3")),
            userId,
            jwtId,
            token3,
            true,
            expirationDate2
        );

        // Refresh token for an access token where the user can't be found
        const string token4 = "90502660-ebbb-405a-9c93-0bd4e9c2ba41-714b9f49-e7b9-4090-8e84-61fdbe8e9f6e";
        const string jwtId2 = "87039e2f-867e-409d-bb60-e8dabc84f52d";
        var invalidUserId = new UserId(Guid.Parse("1e09ca29-2910-4f54-8002-2d9e063090c6"));
        var refreshToken4 = RefreshToken.Create(
            new RefreshTokenId(Guid.Parse("03cb12a5-26f4-4bcd-b6fb-ff5d82f9bf10")),
            invalidUserId,
            jwtId2,
            token4,
            true,
            expirationDate1
        );

        var smartMeter1 = SmartMeter.Create(new SmartMeterId(Guid.Parse("5e9db066-1b47-46cc-bbde-0b54c30167cd")),
            "Smart Meter 1");
        var smartMeter2 = SmartMeter.Create(new SmartMeterId(Guid.Parse("f4c70232-6715-4c15-966f-bf4bcef46d39")),
            "Smart Meter 2");
        var smartMeter2Metadata = Metadata.Create(new MetadataId(Guid.Parse("1c8c8313-6fc4-4ebd-9ca8-8a1267441e06")),
            DateTime.UtcNow, new Location("Some Streetname", "Some city", "Some state", "Some county", Continent.Asia),
            4, smartMeter2.Id);
        smartMeter2.AddMetadata(smartMeter2Metadata);

        await _applicationDbContext.Tenants.AddAsync(tenant);
        await _applicationDbContext.Users.AddAsync(testUser);
        await _applicationDbContext.DomainUsers.AddAsync(domainUser);
        await _applicationDbContext.RefreshTokens.AddAsync(refreshToken1);
        await _applicationDbContext.RefreshTokens.AddAsync(refreshToken2);
        await _applicationDbContext.RefreshTokens.AddAsync(refreshToken3);
        await _applicationDbContext.RefreshTokens.AddAsync(refreshToken4);
        await _tenantDbContext.SmartMeters.AddAsync(smartMeter1);
        await _tenantDbContext.SmartMeters.AddAsync(smartMeter2);

        await _applicationDbContext.SaveChangesAsync();
        await _tenantDbContext.SaveChangesAsync();
    }
}