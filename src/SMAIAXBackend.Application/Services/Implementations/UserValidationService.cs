using Microsoft.Extensions.Logging;

using SMAIAXBackend.Application.Exceptions;
using SMAIAXBackend.Application.Services.Interfaces;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Domain.Repositories;

namespace SMAIAXBackend.Application.Services.Implementations;

public class UserValidationService(IUserRepository userRepository, ILogger<UserValidationService> logger) : IUserValidationService
{
    public async Task<UserId> ValidateUserAsync(string? userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            logger.LogWarning("No user claim found in claims principal.");
            throw new InvalidTokenException();
        }

        if (!Guid.TryParse(userId, out var userIdGuid))
        {
            logger.LogWarning("Invalid user claim found in claims principal.");
            throw new InvalidTokenException();
        }

        var user = await userRepository.GetUserByIdAsync(new UserId(userIdGuid));

        if (user is null)
        {
            logger.LogWarning("User with id {UserId} not found in database.", userIdGuid);
            throw new UserNotFoundException(userIdGuid);
        }

        return new UserId(userIdGuid);
    }
}