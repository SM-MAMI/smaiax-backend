namespace SMAIAXBackend.Application.DTOs;

public class DeviceConfigDto(string encryptedMqttUsername,string encryptedMqttPassword, string topic, string publicKey)
{
    public string EncryptedMqttUsername { get; set; } = encryptedMqttUsername;
    public string EncryptedMqttPassword { get; set; } = encryptedMqttPassword;
    public string Topic { get; set; } = topic;
    public string PublicKey { get; set; } = publicKey;
}