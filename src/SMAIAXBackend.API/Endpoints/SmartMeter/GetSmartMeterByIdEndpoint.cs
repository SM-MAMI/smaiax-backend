using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Services.Interfaces;

namespace SMAIAXBackend.API.Endpoints.SmartMeter;

public static class GetSmartMeterByIdEndpoint
{
    public static async Task<Ok<SmartMeterOverviewDto>> Handle(
        ISmartMeterListService smartMeterListService,
        [FromRoute] Guid id)
    {
        var smartMeter = await smartMeterListService.GetSmartMeterByIdAsync(id);

        return TypedResults.Ok(smartMeter);
    }
}