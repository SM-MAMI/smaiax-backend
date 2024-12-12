using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Services.Interfaces;
using SMAIAXBackend.Domain.Repositories;

namespace SMAIAXBackend.Application.Services.Implementations;

public class DeviceConfigListService(IVaultRepository vaultRepository) : IDeviceConfigListService
{
    public Task<DeviceConfigDto> GetDeviceConfigByDeviceIdAsync(Guid id)
    {
        
        vaultRepository.GetMqttBroker
    }
}