using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Services.Interfaces;

namespace SMAIAXBackend.API.Endpoints.DeviceConfig;

public static class GetDeviceConfigEndpoint
{
    public static async Task<Ok<DeviceConfigDto>> Handle(
        IDeviceConfigListService deviceConfigListService,
        [FromQuery] Guid id)
    {
        var deviceConfig = await deviceConfigListService.GetDeviceConfigByDeviceIdAsync(id);
        return TypedResults.Ok(deviceConfig);
    }
}