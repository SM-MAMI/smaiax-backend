using System.Security.Claims;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Services.Interfaces;

namespace SMAIAXBackend.API.Endpoints.PolicyRequest;

public static class CreatePolicyRequestEndpoint
{
    public static async Task<Ok<Guid>> Handle(
        IPolicyRequestCreateService policyRequestCreateService, [
        FromBody] PolicyRequestCreateDto policyRequestCreateDto,
        ClaimsPrincipal user)
    {
        var userIdClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        var policyRequestId = await policyRequestCreateService.CreatePolicyRequestAsync(policyRequestCreateDto, userIdClaim);

        return TypedResults.Ok(policyRequestId);
    }
}