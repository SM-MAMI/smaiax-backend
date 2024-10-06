using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Exceptions;
using SMAIAXBackend.Application.Services.Interfaces;
using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Repositories;

namespace SMAIAXBackend.Application.Services.Implementations;

public class UserService(
    IUserRepository userRepository,
    ITokenService tokenService,
    UserManager<IdentityUser> userManager,
    ILogger<UserService> logger) : IUserService
{
    public async Task<Guid> RegisterAsync(RegisterDto registerDto)
    {
        var userId = userRepository.NextIdentity();
        var identityUser = new IdentityUser
        {
            Id = userId.Id.ToString(), UserName = registerDto.Email, Email = registerDto.Email
        };

        var result = await userManager.CreateAsync(identityUser, registerDto.Password);

        if (!result.Succeeded)
        {
            var errorMessages = string.Join(", ", result.Errors.Select(e => e.Description));
            logger.LogError("Registration failed with the following errors: {ErrorMessages}", errorMessages);
            throw new RegistrationException(errorMessages);
        }

        var domainUser = User.Create(userId, registerDto.Name, registerDto.Address, registerDto.Email);
        await userRepository.AddAsync(domainUser);

        return userId.Id;
    }

    public async Task<TokenDto> LoginAsync(LoginDto loginDto)
    {
        var user = await userManager.FindByNameAsync(loginDto.Username);

        if (user == null)
        {
            logger.LogError("User with `{Username}` not found.", loginDto.Username);
            throw new InvalidLoginException();
        }

        var isPasswordCorrect = await userManager.CheckPasswordAsync(user, loginDto.Password);

        if (!isPasswordCorrect)
        {
            logger.LogError("Invalid password for user `{Username}`.", loginDto.Username);
            throw new InvalidLoginException();
        }

        // To avoid the null reference warning
        var userName = user.UserName ?? string.Empty;
        var tokenId = tokenService.NextIdentity();
        var accessToken = await tokenService.GenerateAccessTokenAsync(tokenId.ToString(), user.Id, userName);
        var refreshToken = await tokenService.GenerateRefreshTokenAsync(tokenId.ToString(), user.Id);
        var tokenDto = new TokenDto(accessToken, refreshToken.Token);

        return tokenDto;
    }

    public async Task<TokenDto> RefreshTokensAsync(TokenDto tokenDto)
    {
        var existingRefreshToken = await tokenService.GetRefreshTokenByTokenAsync(tokenDto.RefreshToken);

        if (existingRefreshToken == null || !existingRefreshToken.IsValid)
        {
            logger.LogError("Invalid or non-existent refresh token: {RefreshToken}", tokenDto.RefreshToken);
            throw new InvalidTokenException();
        }

        if (existingRefreshToken.ExpiresAt < DateTime.UtcNow)
        {
            existingRefreshToken.Invalidate();
            await tokenService.UpdateAsync(existingRefreshToken);
            logger.LogError("Expired refresh token: {RefreshToken}, Expired at: {ExpiresAt}", tokenDto.RefreshToken,
                existingRefreshToken.ExpiresAt);
            throw new InvalidTokenException();
        }

        var isAccessTokenValid = tokenService.ValidateAccessToken(tokenDto.AccessToken, existingRefreshToken.UserId,
            existingRefreshToken.JwtTokenId);

        if (!isAccessTokenValid)
        {
            existingRefreshToken.Invalidate();
            await tokenService.UpdateAsync(existingRefreshToken);
            logger.LogError("Invalid access token for refresh token: {RefreshToken}, User ID: {UserId}",
                tokenDto.RefreshToken, existingRefreshToken.UserId);
            throw new InvalidTokenException();
        }

        var identityUser = await userManager.FindByIdAsync(existingRefreshToken.UserId.ToString()!);

        if (identityUser == null)
        {
            logger.LogError("User not found for ID: {UserId}", existingRefreshToken.UserId.ToString());
            throw new InvalidTokenException();
        }

        var tokenId = tokenService.NextIdentity();
        var newRefreshToken = await tokenService.GenerateRefreshTokenAsync(tokenId.ToString(),
            existingRefreshToken.UserId.Id.ToString());
        var newAccessToken = await tokenService.GenerateAccessTokenAsync(tokenId.ToString(),
            identityUser.Id, identityUser.UserName!);
        var refreshedTokens = new TokenDto(newAccessToken, newRefreshToken.Token);

        return refreshedTokens;
    }
}