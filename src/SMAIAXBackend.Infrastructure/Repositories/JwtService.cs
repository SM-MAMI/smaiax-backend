using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects;
using SMAIAXBackend.Domain.Repositories;
using SMAIAXBackend.Infrastructure.Configurations;
using SMAIAXBackend.Infrastructure.DbContexts;

namespace SMAIAXBackend.Infrastructure.Repositories;

public class JwtService(IOptions<JwtConfiguration> jwtConfigOptions, UserStoreDbContext userStoreDbContext) : ITokenService
{
    private readonly JwtConfiguration _jwtConfig = jwtConfigOptions.Value;

    public Guid NextIdentity()
    {
        return Guid.NewGuid();
    }

    public Task<string> GenerateAccessTokenAsync(string jwtTokenId, string userId, string username)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.UniqueName, username),
            new Claim(JwtRegisteredClaimNames.Jti, jwtTokenId),
            new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString(CultureInfo.InvariantCulture),
                ClaimValueTypes.Integer64)
        };

        var token = new JwtSecurityToken(
            issuer: _jwtConfig.Issuer,
            audience: _jwtConfig.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtConfig.ExpirationMinutes),
            signingCredentials: credentials
        );

        var tokenString = jwtSecurityTokenHandler.WriteToken(token);

        return Task.FromResult(tokenString);
    }

    public async Task<RefreshToken> GenerateRefreshToken(string jwtTokenId, string userId)
    {
        var refreshTokenId = new RefreshTokenId(Guid.NewGuid());
        var token = $"{Guid.NewGuid()}-{Guid.NewGuid()}";
        var refreshToken = RefreshToken.Create(refreshTokenId, new UserId(Guid.Parse(userId)), jwtTokenId,
            token, true, DateTime.UtcNow.AddMinutes(_jwtConfig.ExpirationMinutes));

        await userStoreDbContext.RefreshTokens.AddAsync(refreshToken);
        await userStoreDbContext.SaveChangesAsync();
        
        return refreshToken;
    }
}