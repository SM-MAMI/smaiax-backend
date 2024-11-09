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
        var smartMeterId = await smartMeterCreateService.AddMetadataAsync(id, metadataCreateDto);
        return TypedResults.Ok(smartMeterId);
    }
}