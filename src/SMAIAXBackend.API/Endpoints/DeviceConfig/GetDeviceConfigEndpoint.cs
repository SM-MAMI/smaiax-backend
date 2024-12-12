using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Services.Interfaces;

namespace SMAIAXBackend.API.Endpoints.DeviceConfig;

public static class GetDeviceConfigEndpoint
{
    public static async Task<Ok<DeviceConfigDto>> Handle(
        ISmartMeterListService smartMeterListService,
        [FromRoute] Guid id)
    {
        var smartMeter = await smartMeterListService.GetSmartMeterByIdAsync(id);

        return TypedResults.Ok(smartMeter);
    }
}