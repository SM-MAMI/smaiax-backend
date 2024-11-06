using System.Security.Claims;

using Microsoft.AspNetCore.Http.HttpResults;

using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Services.Interfaces;

namespace SMAIAXBackend.API.Endpoints.SmartMeter;

public static class GetSmartMetersEndpoint
{
    public static async Task<Ok<List<SmartMeterOverviewDto>>> Handle(
        ISmartMeterListService smartMeterListService,
        ClaimsPrincipal user)
    {
        var userIdClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        var smartMeters = await smartMeterListService.GetSmartMetersByUserIdAsync(userIdClaim);

        return TypedResults.Ok(smartMeters);
    }
}