using Microsoft.Extensions.Logging;

using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Exceptions;
using SMAIAXBackend.Application.Services.Interfaces;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Domain.Repositories;

namespace SMAIAXBackend.Application.Services.Implementations;

public class UserService(IUserRepository userRepository,
    ILogger<SmartMeterListService> logger) : IUserService
{
    public async Task<UserDto> GetUserByIdAsync(Guid userId)
    {
        var user = await userRepository.GetUserByIdAsync(new UserId(userId));
        
        if (user == null)
        {
            logger.LogError("User with id '{UserId} not found.", userId);
            throw new UserNotFoundException(userId);
        }
        
        var userDto = UserDto.FromUser(user);
        
        return userDto;
    }
}