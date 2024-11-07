using System.Security.Claims;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using SMAIAXBackend.Application.Services.Interfaces;

namespace SMAIAXBackend.API.Endpoints.SmartMeter;

public static class RemoveMetadataEndpoint
{
    public static async Task<NoContent> Handle(
        ISmartMeterDeleteService smartMeterDeleteService,
        [FromRoute] Guid smartMeterId,
        [FromRoute] Guid metadataId,
        ClaimsPrincipal user)
    {
        var userIdClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        await smartMeterDeleteService.RemoveMetadataFromSmartMeterAsync(smartMeterId, metadataId, userIdClaim);

        return TypedResults.NoContent();
    }
}