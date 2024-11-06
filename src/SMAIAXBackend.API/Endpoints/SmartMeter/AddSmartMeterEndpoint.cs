using System.Security.Claims;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Services.Interfaces;

namespace SMAIAXBackend.API.Endpoints.SmartMeter;

public static class AddSmartMeterEndpoint
{
    public static async Task<CreatedAtRoute> Handle(
        ISmartMeterCreateService smartMeterCreateService,
        ClaimsPrincipal user,
        [FromBody] SmartMeterCreateDto smartMeterCreateDto)
    {
        var userIdClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        var id = await smartMeterCreateService.AddSmartMeterAsync(smartMeterCreateDto, userIdClaim);

        return TypedResults.CreatedAtRoute("getSmartMeterById", new { id });
    }
}