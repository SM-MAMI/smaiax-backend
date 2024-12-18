using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Services.Interfaces;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Domain.Repositories;

namespace SMAIAXBackend.Application.Services.Implementations;

public class DeviceConfigListService(IVaultRepository vaultRepository) : IDeviceConfigListService
{
    public async Task<DeviceConfigDto> GetDeviceConfigByDeviceIdAsync(Guid id)
    {
        var (username, password, topic) = await vaultRepository.GetMqttBrokerCredentialsAsync(new SmartMeterId(id));
        
        if (username == null || password == null || topic == null)
        {
            //TODO: create custom exception
            throw new InvalidOperationException("Device config not found");
        }
        
        //TODO: create order process to generate public/private key pair
        var deviceConfigDto = new DeviceConfigDto(username, password, topic, "");
        
        return deviceConfigDto;
    }
}