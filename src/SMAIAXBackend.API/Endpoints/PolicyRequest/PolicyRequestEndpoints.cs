using SMAIAXBackend.Application.DTOs;

namespace SMAIAXBackend.API.Endpoints.PolicyRequest;

public static class PolicyRequestEndpoints
{
    public static WebApplication MapPolicyRequestEndpoints(this WebApplication app)
    {
        const string contentType = "application/json";
        var group = app.MapGroup("/api/policyRequests")
            .WithTags("Policyrequest")
            .RequireAuthorization();

        group.MapPost("/", CreatePolicyRequestEndpoint.Handle)
            .WithName("createPolicyRequest")
            .Accepts<PolicyRequestCreateDto>(contentType)
            .Produces<List<PolicyDto>>(StatusCodes.Status200OK, contentType)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("/{id:guid}/policies", GetMatchingPoliciesEndpoint.Handle)
            .WithName("getMatchingPolicies")
            .Accepts<Guid>(contentType)
            .Produces<List<PolicyDto>>(StatusCodes.Status200OK, contentType)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        return app;
    }
}