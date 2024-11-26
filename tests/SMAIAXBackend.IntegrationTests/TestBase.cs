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
    protected readonly TenantDbContext _tenant1DbContext = IntegrationTestSetup.Tenant1DbContext;
    protected readonly TenantDbContext _tenant2DbContext = IntegrationTestSetup.Tenant2DbContext;
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
        await IntegrationTestSetup.TenantRepository.CreateDatabaseForTenantAsync("tenant_2_db", "janedoe", "P@ssw0rd");
        await InsertTestData();
        IntegrationTestSetup.ApplicationDbContext.ChangeTracker.Clear();
        IntegrationTestSetup.Tenant1DbContext.ChangeTracker.Clear();
        IntegrationTestSetup.Tenant2DbContext.ChangeTracker.Clear();
    }

    [TearDown]
    public async Task TearDown()
    {
        IntegrationTestSetup.ApplicationDbContext.ChangeTracker.Clear();
        IntegrationTestSetup.Tenant1DbContext.ChangeTracker.Clear();
        IntegrationTestSetup.Tenant2DbContext.ChangeTracker.Clear();
        await IntegrationTestSetup.Tenant1DbContext.Database.EnsureDeletedAsync();
        await IntegrationTestSetup.Tenant2DbContext.Database.EnsureDeletedAsync();
        await CleanupTenantDatabases();
        await IntegrationTestSetup.ApplicationDbContext.Database.EnsureDeletedAsync();
    }

    private static async Task CleanupTenantDatabases()
    {
        await using var deleteUserCommand = IntegrationTestSetup.ApplicationDbContext.Database.GetDbConnection().CreateCommand();
        deleteUserCommand.CommandText = "DROP ROLE IF EXISTS johndoe; DROP ROLE IF EXISTS janedoe;";
        await IntegrationTestSetup.ApplicationDbContext.Database.OpenConnectionAsync();
        await deleteUserCommand.ExecuteNonQueryAsync();
        await IntegrationTestSetup.ApplicationDbContext.Database.CloseConnectionAsync();
    }

    private async Task InsertTestData()
    {
        var hasher = new PasswordHasher<IdentityUser>();

        var johnDoeUserId = new UserId(Guid.Parse("3c07065a-b964-44a9-9cdf-fbd49d755ea7"));
        const string johnDoeUserName = "johndoe";
        const string johnDoeEmail = "john.doe@example.com";
        const string johnDoePassword = "P@ssw0rd";
        var johnDoeTestUser = new IdentityUser
        {
            Id = johnDoeUserId.Id.ToString(),
            UserName = johnDoeUserName,
            NormalizedUserName = johnDoeUserName.ToUpper(),
            Email = johnDoeEmail,
            NormalizedEmail = johnDoeEmail.ToUpper(),
        };
        var johnDoePasswordHash = hasher.HashPassword(johnDoeTestUser, johnDoePassword);
        johnDoeTestUser.PasswordHash = johnDoePasswordHash;

        var johnDoeTenantId = new TenantId(Guid.Parse("f4c70232-6715-4c15-966f-bf4bcef46d39"));
        var johnDoeTenant = Tenant.Create(johnDoeTenantId, johnDoeUserName, johnDoePassword, "tenant_1_db");
        var johnDoeDomainUser = User.Create(johnDoeUserId, new Name("John", "Doe"), johnDoeUserName, johnDoeEmail, johnDoeTenantId);

        var janeDoeUserId = new UserId(Guid.Parse("4d07065a-b964-44a9-9cdf-fbd49d755ea8"));
        const string janeDoeUserName = "janedoe";
        const string janeDoeEmail = "jane.doe@example.com";
        const string janeDoePassword = "P@ssw0rd";
        var janeDoeTestUser = new IdentityUser
        {
            Id = janeDoeUserId.Id.ToString(),
            UserName = janeDoeUserName,
            NormalizedUserName = janeDoeUserName.ToUpper(),
            Email = janeDoeEmail,
            NormalizedEmail = janeDoeEmail.ToUpper(),
        };
        var janeDoePasswordHash = hasher.HashPassword(janeDoeTestUser, janeDoePassword);
        janeDoeTestUser.PasswordHash = janeDoePasswordHash;

        var janeDoeTenantId = new TenantId(Guid.Parse("e4c70232-6715-4c15-966f-bf4bcef46d40"));
        var janeDoeTenant = Tenant.Create(janeDoeTenantId, janeDoeUserName, janeDoePassword, "tenant_2_db");
        var janeDoeDomainUser = User.Create(janeDoeUserId, new Name("Jane", "Doe"), janeDoeUserName, janeDoeEmail, janeDoeTenantId);

        // Valid refresh token
        const string jwtId = "19f77b2e-e485-4031-8506-62f6d3b69e4d";
        const string token1 = "4dffb63c-581d-4588-8b4b-4b075f17d015-abcb30f4-5f32-4fbb-80c4-99cea98273ca";
        var expirationDate1 = DateTime.UtcNow.AddDays(100);
        var refreshToken1 = RefreshToken.Create(
            new RefreshTokenId(Guid.Parse("21938ead-d43f-4f16-a055-c3b5613cd599")),
            johnDoeUserId,
            jwtId,
            token1,
            true,
            expirationDate1
        );

        // Invalid refresh token
        const string token2 = "266cbbdb-edcd-48a6-aa63-f837b05a2551-3b01aaa3-304a-434b-bc7d-fd9a6305550b";
        var refreshToken2 = RefreshToken.Create(
            new RefreshTokenId(Guid.Parse("ad758462-20de-41fc-91d4-0569466224fc")),
            johnDoeUserId,
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
            johnDoeUserId,
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

        // John Doe
        var smartMeter2Id = new SmartMeterId(Guid.Parse("f4c70232-6715-4c15-966f-bf4bcef46d39"));
        var smartMeter2Metadata = Metadata.Create(new MetadataId(Guid.Parse("1c8c8313-6fc4-4ebd-9ca8-8a1267441e06")),
            DateTime.UtcNow, new Location("Some Streetname", "Some city", "Some state", "Some county", Continent.Asia),
            4, smartMeter2Id);
        var smartMeter1 = SmartMeter.Create(new SmartMeterId(Guid.Parse("5e9db066-1b47-46cc-bbde-0b54c30167cd")),
            "Smart Meter 1", []);
        var smartMeter2 = SmartMeter.Create(smartMeter2Id, "Smart Meter 2", [smartMeter2Metadata]);
        var policyRequest = PolicyRequest.Create(new PolicyRequestId(Guid.Parse("58af578c-9975-4633-8dfe-ff8b70b83661")),
            false, new PolicyFilter(MeasurementResolution.Hour, 1, 10, [],
                LocationResolution.State, 500));

        // Jane Doe
        var smartMeter3Id = new SmartMeterId(Guid.Parse("f4c70232-6715-4c15-966f-bf4bcef46d39"));
        var smartMeter3Metadata = Metadata.Create(new MetadataId(Guid.Parse("1c8c8313-6fc4-4ebd-9ca8-8a1267441e06")),
            DateTime.UtcNow, new Location("Some Streetname", "Some city", "Some state", "Some county", Continent.Asia),
            4, smartMeter3Id);
        var smartMeter3 = SmartMeter.Create(smartMeter3Id,
            "Smart Meter 3", [smartMeter3Metadata]);
        var policy1 = Policy.Create(new PolicyId(Guid.Parse("f4c70232-6715-4c15-966f-bf4bcef46d39")),
            MeasurementResolution.Hour, LocationResolution.Country, 500, smartMeter3Id);
        var policy2 = Policy.Create(new PolicyId(Guid.Parse("a4c70232-6715-4c15-966f-bf4bcef46d40")),
                MeasurementResolution.Minute, LocationResolution.City, 1000, smartMeter3Id);


        await _applicationDbContext.Tenants.AddAsync(johnDoeTenant);
        await _applicationDbContext.Tenants.AddAsync(janeDoeTenant);
        await _applicationDbContext.Users.AddAsync(johnDoeTestUser);
        await _applicationDbContext.Users.AddAsync(janeDoeTestUser);
        await _applicationDbContext.DomainUsers.AddAsync(johnDoeDomainUser);
        await _applicationDbContext.DomainUsers.AddAsync(janeDoeDomainUser);
        await _applicationDbContext.RefreshTokens.AddAsync(refreshToken1);
        await _applicationDbContext.RefreshTokens.AddAsync(refreshToken2);
        await _applicationDbContext.RefreshTokens.AddAsync(refreshToken3);
        await _applicationDbContext.RefreshTokens.AddAsync(refreshToken4);
        await _tenant1DbContext.SmartMeters.AddAsync(smartMeter1);
        await _tenant1DbContext.SmartMeters.AddAsync(smartMeter2);
        await _tenant1DbContext.PolicyRequests.AddAsync(policyRequest);
        await _tenant2DbContext.SmartMeters.AddAsync(smartMeter3);
        await _tenant2DbContext.Policies.AddAsync(policy1);
        await _tenant2DbContext.Policies.AddAsync(policy2);

        await _applicationDbContext.SaveChangesAsync();
        await _tenant1DbContext.SaveChangesAsync();
        await _tenant2DbContext.SaveChangesAsync();
    }
}