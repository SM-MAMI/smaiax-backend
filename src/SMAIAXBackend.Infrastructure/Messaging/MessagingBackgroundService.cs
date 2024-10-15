using Microsoft.Extensions.Hosting;
using SMAIAXBackend.Application.Interfaces;

namespace SMAIAXBackend.Infrastructure.Messaging;

public class MessagingBackgroundService(IMqttReader mqttReader) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await mqttReader.ConnectAndSubscribeAsync();

            // Keep the service running until the token is canceled
            while (!stoppingToken.IsCancellationRequested)
            {
                // Perform other tasks, if necessary, or just wait
                await Task.Delay(1000, stoppingToken); // 1-second delay to keep the loop alive
            }
        }
        finally
        {
            await mqttReader.DisconnectAsync();
        }
    }
}