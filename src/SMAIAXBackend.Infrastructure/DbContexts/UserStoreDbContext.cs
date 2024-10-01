using Microsoft.EntityFrameworkCore;

namespace SMAIAXBackend.Infrastructure.DbContexts;

public class UserStoreDbContext(DbContextOptions<UserStoreDbContext> options) : DbContext(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseCamelCaseNamingConvention();
    }
}