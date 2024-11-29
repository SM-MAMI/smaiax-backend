using System.Net.Http.Headers;
using System.Text;

using Microsoft.Extensions.Options;

using Newtonsoft.Json;

using SMAIAXBackend.Domain.Repositories;
using SMAIAXBackend.Infrastructure.Configurations;

namespace SMAIAXBackend.Infrastructure.Repositories;

public class RabbitMqRepository : IMqttBrokerRepository
{
    private readonly HttpClient _httpClient;

    public RabbitMqRepository(IOptions<MqttBrokerConfiguration> mqttBrokerConfigOptions)
    {
        _httpClient = new HttpClient();
        _httpClient.BaseAddress =
            new Uri($"http://{mqttBrokerConfigOptions.Value.Host}:{mqttBrokerConfigOptions.Value.ManagementPort}/api/");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
            Convert.ToBase64String(
                Encoding.ASCII.GetBytes(
                    $"{mqttBrokerConfigOptions.Value.Username}:{mqttBrokerConfigOptions.Value.Password}")));
    }

    public async Task CreateMqttUserAsync(string topic, string username, string password)
    {
        var createUserResponse = await _httpClient.PutAsync($"users/{username}", new StringContent(
            JsonConvert.SerializeObject(new { password, tags = "" }), Encoding.UTF8, "application/json"));

        createUserResponse.EnsureSuccessStatusCode();

        // TODO: Restrict permissions to topic
        var permissionsPayload = new
        {
            configure = ".*",
            write = ".*",
            read = ".*"
        };

        var permissionsResponse = await _httpClient.PutAsync(
            $"permissions/%2F/{username}",
            new StringContent(JsonConvert.SerializeObject(permissionsPayload), Encoding.UTF8, "application/json"));

        permissionsResponse.EnsureSuccessStatusCode();
    }
}