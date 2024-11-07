using System.Diagnostics.CodeAnalysis;

using Microsoft.EntityFrameworkCore;

using Npgsql;

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
            var connectionStringBuilder = new NpgsqlConnectionStringBuilder
            {
                Host = dbConfig!.Host,
                Port = dbConfig.Port,
                Username = dbConfig.SuperUsername,
                Password = dbConfig.SuperUserPassword,
                Database = dbConfig.MainDatabase
            };
            options.UseNpgsql(connectionStringBuilder.ConnectionString);
        });
        
        // This is needed to create migrations for the TenantDbContext
        services.AddDbContext<TenantDbContext>(options =>
        {
            var dbConfig = configuration.GetSection("DatabaseConfiguration").Get<DatabaseConfiguration>();
            var connectionStringBuilder = new NpgsqlConnectionStringBuilder
            {
                Host = dbConfig!.Host,
                Port = dbConfig.Port,
                Username = dbConfig.SuperUsername,
                Password = dbConfig.SuperUserPassword,
                Database = "tenant_template_db"
            };
            options.UseNpgsql(connectionStringBuilder.ConnectionString);
        });
        
        services.AddScoped<ITenantDbContextFactory, TenantDbContextFactory>();
    }
}