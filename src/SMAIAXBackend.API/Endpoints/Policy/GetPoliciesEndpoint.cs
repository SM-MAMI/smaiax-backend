using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Services.Interfaces;
using SMAIAXBackend.Domain.Model.Enums;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.API.Endpoints.Policy;

public static class GetPoliciesEndpoint
{
    public static async Task<Results<Ok<List<PolicyDto>>, NotFound>> Handle(
        IPolicyListService policyListService,
        [FromQuery] Guid? smartMeterId,
        [FromQuery] decimal? maxPrice,
        [FromQuery] MeasurementResolution? measurementResolution)
    {
        List<PolicyDto> policies;

        if (smartMeterId.HasValue)
        {
            policies = await policyListService.GetPoliciesBySmartMeterIdAsync(new SmartMeterId(smartMeterId.Value));
        }
        else if (maxPrice.HasValue || measurementResolution.HasValue)
        {
            // In this case a user is searching for policies of other users based on filters
            policies = await policyListService.GetFilteredPoliciesAsync(maxPrice, measurementResolution);
        }
        else
        {
            policies = await policyListService.GetPoliciesAsync();
        }

        return TypedResults.Ok(policies);
    }
}