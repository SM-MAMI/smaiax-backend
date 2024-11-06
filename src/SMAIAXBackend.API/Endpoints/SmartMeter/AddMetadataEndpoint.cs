using System.Security.Claims;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Services.Interfaces;

namespace SMAIAXBackend.API.Endpoints.SmartMeter;

public static class AddMetadataEndpoint
{
    public static async Task<Ok<Guid>> Handle(
        ISmartMeterCreateService smartMeterCreateService,
        [FromRoute] Guid id,
        [FromBody] MetadataCreateDto metadataCreateDto,
        ClaimsPrincipal user)
    {
        var userIdClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        var smartMeterId = await smartMeterCreateService.AddMetadataAsync(id, metadataCreateDto, userIdClaim);
        return TypedResults.Ok(smartMeterId);
    }
}