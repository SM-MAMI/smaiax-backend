using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects;
using SMAIAXBackend.Infrastructure.DbContexts;

namespace SMAIAXBackend.IntegrationTests;

public class TestBase
{
    protected readonly HttpClient HttpClient = IntegrationTestSetup.HttpClient;
    protected readonly ApplicationDbContext ApplicationDbContext = IntegrationTestSetup.ApplicationDbContext;

    [SetUp]
    public async Task Setup()
    {
        await IntegrationTestSetup.ApplicationDbContext.Database.EnsureCreatedAsync();
        await InsertTestData();
        IntegrationTestSetup.ApplicationDbContext.ChangeTracker.Clear();
    }

    [TearDown]
    public async Task TearDown()
    {
        await IntegrationTestSetup.ApplicationDbContext.Database.EnsureDeletedAsync();
    }

    private async Task InsertTestData()
    {
        // Valid refresh token
        var userId = new UserId(Guid.Parse("3c07065a-b964-44a9-9cdf-fbd49d755ea7"));
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

        await ApplicationDbContext.RefreshTokens.AddAsync(refreshToken1);
        await ApplicationDbContext.RefreshTokens.AddAsync(refreshToken2);
        await ApplicationDbContext.RefreshTokens.AddAsync(refreshToken3);
        await ApplicationDbContext.RefreshTokens.AddAsync(refreshToken4);
        await ApplicationDbContext.SaveChangesAsync();
    }
}