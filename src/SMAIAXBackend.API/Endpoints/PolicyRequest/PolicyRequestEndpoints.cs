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
            .Produces<Guid>(StatusCodes.Status200OK, contentType)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        return app;
    }
}