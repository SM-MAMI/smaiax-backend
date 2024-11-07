using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.Domain.Repositories;

public interface IUserRepository
{
    UserId NextIdentity();
    Task AddAsync(User user);
    Task<User?> GetUserByIdAsync(UserId userId);
    Task DeleteAsync(User user);
}