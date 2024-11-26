using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Services.Interfaces;

namespace SMAIAXBackend.API.Endpoints.PolicyRequest;

public static class GetMatchingPoliciesEndpoint
{
    public static async Task<Ok<List<PolicyDto>>> Handle(IPolicyMatchingService policyMatchingService, [FromRoute] Guid id)
    {
        var policies = await policyMatchingService.GetMatchingPoliciesAsync(id);

        return TypedResults.Ok(policies);
    }
}