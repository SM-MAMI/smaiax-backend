using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Services.Interfaces;

namespace SMAIAXBackend.API.Endpoints.PolicyRequest;

public static class CreatePolicyRequestEndpoint
{
    public static async Task<Ok<List<PolicyDto>>> Handle(
        IPolicyRequestCreateService policyRequestCreateService,
        [FromBody] PolicyRequestCreateDto policyRequestCreateDto)
    {
        var matchedPolicies = await policyRequestCreateService.CreatePolicyRequestAsync(policyRequestCreateDto);

        return TypedResults.Ok(matchedPolicies);
    }
}