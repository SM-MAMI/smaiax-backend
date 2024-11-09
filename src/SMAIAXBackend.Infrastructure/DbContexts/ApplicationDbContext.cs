using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Infrastructure.EntityConfigurations;

namespace SMAIAXBackend.Infrastructure.DbContexts;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<IdentityUser>(options)
{
    public DbSet<User> DomainUsers { get; init; }
    public DbSet<RefreshToken> RefreshTokens { get; init; }
    public DbSet<SmartMeter> SmartMeters { get; init; }
    public DbSet<Policy> Policies { get; init; }
    public DbSet<PolicyRequest> PolicyRequests { get; init; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseCamelCaseNamingConvention();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfiguration(new ContractConfiguration());
        builder.ApplyConfiguration(new DomainUserConfiguration());
        builder.ApplyConfiguration(new MeasurementConfiguration());
        builder.ApplyConfiguration(new MetadataConfiguration());
        builder.ApplyConfiguration(new PolicyConfiguration());
        builder.ApplyConfiguration(new PolicyRequestConfiguration());
        builder.ApplyConfiguration(new RefreshTokenConfiguration());
        builder.ApplyConfiguration(new SmartMeterConfiguration());

        // Place Identity tables in the "auth" schema
        builder.Entity<IdentityUser>(entity => entity.ToTable(name: "AspNetUsers", schema: "auth"));
        builder.Entity<IdentityRole>(entity => entity.ToTable(name: "AspNetRoles", schema: "auth"));
        builder.Entity<IdentityUserRole<string>>(entity => entity.ToTable("AspNetUserRoles", schema: "auth"));
        builder.Entity<IdentityUserClaim<string>>(entity => entity.ToTable("AspNetUserClaims", schema: "auth"));
        builder.Entity<IdentityUserLogin<string>>(entity => entity.ToTable("AspNetUserLogins", schema: "auth"));
        builder.Entity<IdentityRoleClaim<string>>(entity => entity.ToTable("AspNetRoleClaims", schema: "auth"));
        builder.Entity<IdentityUserToken<string>>(entity => entity.ToTable("AspNetUserTokens", schema: "auth"));
    }

    public async Task SeedTestData()
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

        var domainUser = User.Create(userId, new Name("John", "Doe"), userName);

        var smartMeter1 = SmartMeter.Create(new SmartMeterId(Guid.NewGuid()), "Smart Meter 1", domainUser.Id);
        var smartMeter2 = SmartMeter.Create(new SmartMeterId(Guid.NewGuid()), "Smart Meter 2", domainUser.Id);

        await Users.AddAsync(testUser);
        await DomainUsers.AddAsync(domainUser);
        await SmartMeters.AddAsync(smartMeter1);
        await SmartMeters.AddAsync(smartMeter2);
        await SaveChangesAsync();
    }
}