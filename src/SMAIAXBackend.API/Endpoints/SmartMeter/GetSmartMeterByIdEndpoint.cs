using System.Security.Claims;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Services.Interfaces;

namespace SMAIAXBackend.API.Endpoints.SmartMeter;

public static class GetSmartMeterByIdEndpoint
{
    public static async Task<Ok<SmartMeterOverviewDto>> Handle(
        ISmartMeterListService smartMeterListService,
        ClaimsPrincipal user,
        [FromRoute] Guid id)
    {
        var userIdClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        var smartMeter = await smartMeterListService.GetSmartMeterByIdAndUserIdAsync(id, userIdClaim);

        return TypedResults.Ok(smartMeter);
    }
}