namespace SMAIAXBackend.Application.Interfaces;

// TODO: Remove this interface
public interface IMqttReader
{
    Task ConnectAndSubscribeAsync();
    Task DisconnectAsync();
}