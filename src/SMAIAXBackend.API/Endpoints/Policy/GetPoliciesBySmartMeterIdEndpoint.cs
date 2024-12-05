using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Services.Interfaces;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.API.Endpoints.Policy;

public static class GetPoliciesBySmartMeterIdEndpoint
{
    public static async Task<Results<Ok<List<PolicyDto>>, NotFound>> Handle(
        IPolicyListService policyListService,
        [FromQuery] Guid? smartMeterId)
    {
        List<PolicyDto> policies;

        if (smartMeterId.HasValue)
        {
            policies = await policyListService.GetPoliciesBySmartMeterIdAsync(new SmartMeterId(smartMeterId.Value));
        }
        else
        {
            policies = await policyListService.GetPoliciesAsync();
        }

        return TypedResults.Ok(policies);
    }
}