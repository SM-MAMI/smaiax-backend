using SMAIAXBackend.Domain.Model.Entities;

namespace SMAIAXBackend.Domain.Repositories;

public interface ITokenService
{
    Guid NextIdentity();
    Task<string> GenerateAccessTokenAsync(string jwtTokenId, string userId, string username);
    Task<RefreshToken> GenerateRefreshToken(string jwtTokenId, string userId);
}