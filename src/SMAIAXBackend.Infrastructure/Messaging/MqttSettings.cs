namespace SMAIAXBackend.Infrastructure.Messaging;

public class MqttSettings
{
    public required string Broker { get; init; }
    public required int Port { get; init; }
    public required string ClientId { get; init; }
    public required string Username { get; init; }
    public required string Password { get; init; }
    public required string Topic { get; init; }
}