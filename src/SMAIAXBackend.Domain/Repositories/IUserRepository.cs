using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects;

namespace SMAIAXBackend.Domain.Repositories;

public interface IUserRepository
{
    UserId NextIdentity();
    Task AddAsync(User user);
}