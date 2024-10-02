namespace SMAIAXBackend.Domain.Repositories;

public interface ITokenService
{
    Task<string> GenerateAccessTokenAsync(string userId, string username);
}