using SMAIAXBackend.Domain.Repositories;
using SMAIAXBackend.Infrastructure.Configurations;
using SMAIAXBackend.Infrastructure.Repositories;

namespace SMAIAXBackend.API.ApplicationConfigurations;

public static class ExternalServiceExtensions
{
    public static void AddExternalServiceConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<VaultConfiguration>(configuration.GetSection("Vault"));
        services.AddSingleton<IVaultService, VaultService>();
        
        services.Configure<MqttBrokerConfiguration>(configuration.GetSection("MqttBroker"));
        services.AddScoped<IMqttBrokerRepository, RabbitMqRepository>();
    }
}