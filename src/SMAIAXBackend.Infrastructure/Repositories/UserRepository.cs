using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects;
using SMAIAXBackend.Domain.Repositories;
using SMAIAXBackend.Infrastructure.DbContexts;

namespace SMAIAXBackend.Infrastructure.Repositories;

public class UserRepository(ApplicationDbContext applicationDbContext) : IUserRepository
{
    public UserId NextIdentity()
    {
        return new UserId(Guid.NewGuid());
    }

    public async Task AddAsync(User user)
    {
        await applicationDbContext.DomainUsers.AddAsync(user);
        await applicationDbContext.SaveChangesAsync();
    }
}