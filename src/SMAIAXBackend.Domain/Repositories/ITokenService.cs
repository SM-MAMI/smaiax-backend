using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects;

namespace SMAIAXBackend.Domain.Repositories;

public interface ITokenService
{
    Guid NextIdentity();
    Task<string> GenerateAccessTokenAsync(string jwtTokenId, string userId, string username);
    Task<RefreshToken> GenerateRefreshTokenAsync(string jwtTokenId, string userId);
    Task<RefreshToken?> GetRefreshTokenByTokenAsync(string token);
    bool ValidateAccessToken(string accessToken, UserId expectedUserId, string expectedJwtTokenId);
    Task UpdateAsync(RefreshToken refreshToken);
}