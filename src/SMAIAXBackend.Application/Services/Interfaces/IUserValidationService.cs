using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.Application.Services.Interfaces;

public interface IUserValidationService
{
    Task<UserId> ValidateUserAsync(string? userId);
}