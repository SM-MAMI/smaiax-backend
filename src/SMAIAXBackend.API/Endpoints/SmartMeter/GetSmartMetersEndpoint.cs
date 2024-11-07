using Microsoft.AspNetCore.Http.HttpResults;

using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Services.Interfaces;

namespace SMAIAXBackend.API.Endpoints.SmartMeter;

public static class GetSmartMetersEndpoint
{
    public static async Task<Ok<List<SmartMeterOverviewDto>>> Handle(ISmartMeterListService smartMeterListService)
    {
        var smartMeters = await smartMeterListService.GetSmartMetersAsync();

        return TypedResults.Ok(smartMeters);
    }
}