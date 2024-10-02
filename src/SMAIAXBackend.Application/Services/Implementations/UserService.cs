using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Exceptions;
using SMAIAXBackend.Application.Services.Interfaces;
using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects;
using SMAIAXBackend.Domain.Repositories;

namespace SMAIAXBackend.Application.Services.Implementations;

public class UserService(
    IUserRepository userRepository,
    UserManager<IdentityUser> userManager,
    ILogger<UserService> logger) : IUserService
{
    public async Task<Guid> Register(RegisterDto registerDto)
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

        var name = new Name(registerDto.FirstName, registerDto.LastName);
        var address = new Address(
            registerDto.Street,
            registerDto.City,
            registerDto.State,
            registerDto.ZipCode,
            registerDto.Country
        );

        var domainUser = User.Create(userId, name, address, registerDto.Email);
        await userRepository.Add(domainUser);

        return userId.Id;
    }
}