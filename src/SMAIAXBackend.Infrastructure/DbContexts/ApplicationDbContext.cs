using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Infrastructure.EntityConfigurations;

namespace SMAIAXBackend.Infrastructure.DbContexts;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<IdentityUser>(options)
{
    public DbSet<User> DomainUsers { get; init; }
    public DbSet<RefreshToken> RefreshTokens { get; init; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseCamelCaseNamingConvention();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfiguration(new DomainUserConfiguration());
        builder.ApplyConfiguration(new RefreshTokenConfiguration());

        // Place Identity tables in the "auth" schema
        builder.Entity<IdentityUser>(entity => entity.ToTable(name: "AspNetUsers", schema: "auth"));
        builder.Entity<IdentityRole>(entity => entity.ToTable(name: "AspNetRoles", schema: "auth"));
        builder.Entity<IdentityUserRole<string>>(entity => entity.ToTable("AspNetUserRoles", schema: "auth"));
        builder.Entity<IdentityUserClaim<string>>(entity => entity.ToTable("AspNetUserClaims", schema: "auth"));
        builder.Entity<IdentityUserLogin<string>>(entity => entity.ToTable("AspNetUserLogins", schema: "auth"));
        builder.Entity<IdentityRoleClaim<string>>(entity => entity.ToTable("AspNetRoleClaims", schema: "auth"));
        builder.Entity<IdentityUserToken<string>>(entity => entity.ToTable("AspNetUserTokens", schema: "auth"));

        SeedTestData(builder);
    }

    private static void SeedTestData(ModelBuilder builder)
    {
        var hasher = new PasswordHasher<IdentityUser>();

        var userId = new UserId(Guid.Parse("3c07065a-b964-44a9-9cdf-fbd49d755ea7"));
        const string userName = "john.doe@example.com";
        var testUser = new IdentityUser
        {
            Id = userId.Id.ToString(),
            UserName = userName,
            NormalizedUserName = userName.ToUpper(),
            Email = userName,
            NormalizedEmail = userName.ToUpper(),
        };
        var passwordHash = hasher.HashPassword(testUser, "P@ssw0rd");
        testUser.PasswordHash = passwordHash;

        builder.Entity<IdentityUser>().HasData(testUser);
    }
}