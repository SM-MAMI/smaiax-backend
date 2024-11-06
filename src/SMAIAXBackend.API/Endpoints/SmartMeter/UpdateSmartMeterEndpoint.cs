using System.Security.Claims;

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
        [FromBody] SmartMeterUpdateDto smartMeterUpdateDto,
        ClaimsPrincipal user)
    {
        var userIdClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        var smartMeterId = await smartMeterUpdateService.UpdateSmartMeterAsync(id, smartMeterUpdateDto, userIdClaim);

        return TypedResults.Ok(smartMeterId);
    }
}