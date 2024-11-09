using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Services.Interfaces;

namespace SMAIAXBackend.API.Endpoints.PolicyRequest;

public static class CreatePolicyRequestEndpoint
{
    public static async Task<Ok<Guid>> Handle(
        IPolicyRequestCreateService policyRequestCreateService,
        [FromBody] PolicyRequestCreateDto policyRequestCreateDto)
    {
        var policyRequestId = await policyRequestCreateService.CreatePolicyRequestAsync(policyRequestCreateDto);

        return TypedResults.Ok(policyRequestId);
    }
}