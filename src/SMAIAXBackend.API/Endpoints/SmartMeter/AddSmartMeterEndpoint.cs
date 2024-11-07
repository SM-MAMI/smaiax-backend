using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Services.Interfaces;

namespace SMAIAXBackend.API.Endpoints.SmartMeter;

public static class AddSmartMeterEndpoint
{
    public static async Task<CreatedAtRoute> Handle(
        ISmartMeterCreateService smartMeterCreateService,
        [FromBody] SmartMeterCreateDto smartMeterCreateDto)
    {
        var id = await smartMeterCreateService.AddSmartMeterAsync(smartMeterCreateDto);

        return TypedResults.CreatedAtRoute("getSmartMeterById", new { id });
    }
}