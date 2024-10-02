using SMAIAXBackend.Domain.Repositories;

namespace SMAIAXBackend.Infrastructure.Repositories;

public class JwtService : ITokenService
{
    public Task<string> GenerateAccessTokenAsync(string userId, string username)
    {
        // TODO: Implement generation of jwt token
        return Task.FromResult("JWT-TOKEN");
    }
}