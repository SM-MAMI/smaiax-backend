namespace SMAIAXBackend.Domain.Repositories;

public interface IMqttBrokerRepository
{
    Task CreateMqttUserAsync(string topic, string username, string password);
}