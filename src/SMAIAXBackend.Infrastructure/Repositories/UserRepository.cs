using Microsoft.EntityFrameworkCore;

using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
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

    public async Task<User?> GetByIdAsync(UserId userId)
    {
        return await applicationDbContext.DomainUsers.FirstOrDefaultAsync(u => u.Id.Equals(userId));
    }
}