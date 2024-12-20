namespace SMAIAXBackend.Infrastructure.Configurations;

public class MqttBrokerConfiguration
{
    public required string Host { get; init; }
    public required int ManagementPort { get; init; }
    public required string Username { get; init; }
    public required string Password { get; init; }
}