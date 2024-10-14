namespace SMAIAXBackend.Infrastructure;

public class MqttSettings(string broker, int port, string clientId, string username, string password, string topic)
{
    public string Broker { get; set; } = broker;
    public int Port { get; set; } = port;
    public string ClientId { get; set; } = clientId;
    public string Username { get; set; } = username;
    public string Password { get; set; } = password;
    public string Topic { get; set; } = topic;
}