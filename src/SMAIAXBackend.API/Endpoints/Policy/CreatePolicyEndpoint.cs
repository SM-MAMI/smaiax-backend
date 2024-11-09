using System.Security.Claims;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Services.Interfaces;

namespace SMAIAXBackend.API.Endpoints.Policy;

public static class CreatePolicyEndpoint
{
    public static async Task<Ok<Guid>> Handle(
        IPolicyCreateService policyCreateService,
        [FromBody] PolicyCreateDto policyCreateDto,
        ClaimsPrincipal user)
    {
        var userIdClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        var policyId = await policyCreateService.CreatePolicyAsync(policyCreateDto, userIdClaim);

        return TypedResults.Ok(policyId);
    }
}