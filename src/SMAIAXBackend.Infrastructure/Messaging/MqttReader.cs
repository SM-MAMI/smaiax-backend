using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using MQTTnet;
using MQTTnet.Client;
using SMAIAXBackend.Application.Interfaces;

namespace SMAIAXBackend.Infrastructure.Messaging;

public class MqttReader : IMqttReader
{
    private readonly MqttSettings _mqttSettings;
    private IMqttClient? _mqttClient;

    public MqttReader(IConfiguration configuration)
    {
        _mqttSettings = configuration.GetSection("MQTT").Get<MqttSettings>();
    }

    public async Task ConnectAndSubscribeAsync()
    {
        var factory = new MqttFactory();
        _mqttClient = factory.CreateMqttClient();

        var options = new MqttClientOptionsBuilder()
            .WithClientId(_mqttSettings.ClientId)
            .WithTcpServer(_mqttSettings.Broker, _mqttSettings.Port)
            .WithCredentials(_mqttSettings.Username, _mqttSettings.Password)
            .WithCleanSession()
            .Build();

        // Connect to MQTT broker
        _mqttClient.ConnectedAsync += async e =>
        {
            Console.WriteLine("Connected to MQTT broker!");

            // Subscribe to the topic
            var topicFilter = new MqttTopicFilterBuilder()
                .WithTopic(_mqttSettings.Topic)
                .Build();

            await _mqttClient.SubscribeAsync(topicFilter);
            Console.WriteLine($"Subscribed to topic {_mqttSettings.Topic}");
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