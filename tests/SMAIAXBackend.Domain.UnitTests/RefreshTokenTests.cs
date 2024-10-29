using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.Domain.UnitTests;

[TestFixture]
public class RefreshTokenTests
{
    [Test]
    public void GivenRefreshTokenDetails_WhenCreateRefreshToken_ThenDetailsEquals()
    {
        // Given
        var refreshTokenId = new RefreshTokenId(Guid.NewGuid());
        var userId = new UserId(Guid.NewGuid());
        const string jwtTokenId = "jwtTokenId";
        const string token = "token";
        const bool isValid = true;
        var expiresAt = DateTime.UtcNow;

        // When
        var refreshToken = RefreshToken.Create(refreshTokenId, userId, jwtTokenId, token, isValid, expiresAt);

        // Then
        Assert.Multiple(() =>
        {
            Assert.That(refreshToken.Id, Is.EqualTo(refreshTokenId));
            Assert.That(refreshToken.UserId, Is.EqualTo(userId));
            Assert.That(refreshToken.JwtTokenId, Is.EqualTo(jwtTokenId));
            Assert.That(refreshToken.Token, Is.EqualTo(token));
            Assert.That(refreshToken.IsValid, Is.EqualTo(isValid));
            Assert.That(refreshToken.ExpiresAt, Is.EqualTo(expiresAt));
        });
    }
}