using SMAIAXBackend.Domain.Repositories;
using SMAIAXBackend.Domain.Repositories.Transactions;
using SMAIAXBackend.Infrastructure;
using SMAIAXBackend.Infrastructure.Repositories;

namespace SMAIAXBackend.API.ApplicationConfigurations;

public static class RepositoryExtensions
{
    public static void AddRepositoryConfigurations(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITokenRepository, TokenRepository>();
        services.AddScoped<ITransactionManager, TransactionManager>();
        services.AddScoped<ISmartMeterRepository, SmartMeterRepository>();
    }
}