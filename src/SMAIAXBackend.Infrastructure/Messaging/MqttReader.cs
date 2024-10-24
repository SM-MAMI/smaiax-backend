using System.Text;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;
using SMAIAXBackend.Application.Interfaces;

namespace SMAIAXBackend.Infrastructure.Messaging;

public class MqttReader(IOptions<MqttSettings> mqttSettings) : IMqttReader
{
    private IMqttClient? _mqttClient;

    public async Task ConnectAndSubscribeAsync()
    {
        var factory = new MqttFactory();
        _mqttClient = factory.CreateMqttClient();

        var options = new MqttClientOptionsBuilder()
            .WithClientId(mqttSettings.Value.ClientId)
            .WithTcpServer(mqttSettings.Value.Broker, mqttSettings.Value.Port)
            .WithCredentials(mqttSettings.Value.Username, mqttSettings.Value.Password)
            .WithCleanSession()
            .Build();

        // Connect to MQTT broker
        _mqttClient.ConnectedAsync += async e =>
        {
            Console.WriteLine("Connected to MQTT broker!");

            // Subscribe to the topic
            var topicFilter = new MqttTopicFilterBuilder()
                .WithTopic(mqttSettings.Value.Topic)
                .Build();

            await _mqttClient.SubscribeAsync(topicFilter);
            Console.WriteLine($"Subscribed to topic {mqttSettings.Value.Topic}");
        };

        // Handle disconnection
        _mqttClient.DisconnectedAsync += e =>
        {
            Console.WriteLine("Disconnected from MQTT broker.");
            return Task.CompletedTask;
        };

        // Handle received messages
        _mqttClient.ApplicationMessageReceivedAsync += e =>
        {
            string message = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment.ToArray());
            Console.WriteLine($"Received message: {message} on topic: {e.ApplicationMessage.Topic}");
            return Task.CompletedTask;
        };

        // Start connection
        await _mqttClient.ConnectAsync(options);
    }

    public async Task DisconnectAsync()
    {
        if (_mqttClient != null && _mqttClient.IsConnected)
        {
            await _mqttClient.DisconnectAsync();
            Console.WriteLine("Disconnected from MQTT broker.");
        }
    }
}