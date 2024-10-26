using SMAIAXBackend.Application.Interfaces;
using SMAIAXBackend.Infrastructure.Messaging;

namespace SMAIAXBackend.API.ApplicationConfigurations;

public static class MqttServiceExtensions
{
    public static void AddMqttServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MqttSettings>(configuration.GetSection("MQTT"));
        services.AddSingleton<IMqttReader, MqttReader>();
        services.AddHostedService<MessagingBackgroundService>();
    }
}