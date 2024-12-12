using SMAIAXBackend.Application.DTOs;

namespace SMAIAXBackend.Application.Services.Interfaces;

public interface IDeviceConfigListService
{
    Task<DeviceConfigDto> GetDeviceConfigByDeviceIdAsync(Guid id);
}