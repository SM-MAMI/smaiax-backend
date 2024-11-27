using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Services.Interfaces;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.API.Endpoints.Policy;

public class GetPoliciesBySmartMeterIdEndpoint
{
    public static async Task<Results<Ok<List<PolicyDto>>, NotFound>> Handle(
        IPolicyListService policyListService,
        [FromQuery] Guid? smartMeterId)
    {
        List<PolicyDto> policies = new List<PolicyDto>();

        if (smartMeterId.HasValue)
        {
            policies = await policyListService.GetPoliciesBySmartMeterIdAsync(new SmartMeterId(smartMeterId.Value));
        }
        else
        {
            // policies = await policyListService.GetAllPoliciesAsync();
        }

        if (policies == null || policies.Count == 0)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(policies);
    }
}