namespace SMAIAXBackend.Application.Interfaces;

public interface IMqttReader
{
    Task ConnectAndSubscribeAsync();
    Task DisconnectAsync();
}