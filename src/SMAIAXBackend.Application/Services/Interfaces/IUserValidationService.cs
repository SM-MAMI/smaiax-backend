using SMAIAXBackend.Domain.Model.Entities;

namespace SMAIAXBackend.Application.Services.Interfaces;

public interface IUserValidationService
{
    Task<User> ValidateUserAsync(string? userId);
}