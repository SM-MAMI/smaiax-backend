using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Services.Interfaces;

namespace SMAIAXBackend.API.Endpoints.SmartMeter;

public static class UpdateSmartMeterEndpoint
{
    public static async Task<Ok<Guid>> Handle(
        ISmartMeterUpdateService smartMeterUpdateService,
        [FromRoute] Guid id,
        [FromBody] SmartMeterUpdateDto smartMeterUpdateDto)
    {
        var smartMeterId = await smartMeterUpdateService.UpdateSmartMeterAsync(id, smartMeterUpdateDto);

        return TypedResults.Ok(smartMeterId);
    }
}