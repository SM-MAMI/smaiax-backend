using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects;
using SMAIAXBackend.Domain.Repositories;
using SMAIAXBackend.Infrastructure.DbContexts;

namespace SMAIAXBackend.Infrastructure.Repositories;

public class UserRepository(UserStoreDbContext userStoreDbContext) : IUserRepository
{
    public UserId NextIdentity()
    {
        return new UserId(Guid.NewGuid());
    }

    public async Task AddAsync(User user)
    {
        await userStoreDbContext.DomainUsers.AddAsync(user);
        await userStoreDbContext.SaveChangesAsync();
    }
}