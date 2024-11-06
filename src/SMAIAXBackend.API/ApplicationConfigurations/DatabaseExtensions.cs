using System.Diagnostics.CodeAnalysis;

using Microsoft.EntityFrameworkCore;

using SMAIAXBackend.Infrastructure.Configurations;
using SMAIAXBackend.Infrastructure.DbContexts;

namespace SMAIAXBackend.API.ApplicationConfigurations;

[ExcludeFromCodeCoverage]
public static class DatabaseExtensions
{
    public static void AddDatabaseConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DatabaseConfiguration>(configuration.GetSection("DatabaseConfiguration"));
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            var dbConfig = configuration.GetSection("DatabaseConfiguration").Get<DatabaseConfiguration>();
            var connectionString = $"Host={dbConfig.Host};Port={dbConfig.Port};Username={dbConfig.SuperUsername};Password={dbConfig.SuperUserPassword};Database={dbConfig.MainDatabase}";
            options.UseNpgsql(connectionString);
        });

        services.AddDbContext<TenantDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("tenant-db"));
        });

        services.AddScoped<ITenantDbContextFactory, TenantDbContextFactory>();
    }
}