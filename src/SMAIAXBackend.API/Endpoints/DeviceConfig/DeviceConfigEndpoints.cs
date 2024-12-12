using SMAIAXBackend.API.Endpoints.Policy;
using SMAIAXBackend.Application.DTOs;

namespace SMAIAXBackend.API.Endpoints.DeviceConfig;

public static class  DeviceConfigEndpoints
{
    public static WebApplication MapDeviceConfigEndpoints(this WebApplication app)
    {
        const string contentType = "application/json";
        var group = app.MapGroup("/api/deviceConfig")
            .WithTags("Policy")
            .RequireAuthorization();

        // group.MapPost("/", CreatePolicyEndpoint.Handle)
        //     .WithName("createPolicy")
        //     .Accepts<PolicyCreateDto>(contentType)
        //     .Produces<Guid>(StatusCodes.Status200OK, contentType)
        //     .ProducesProblem(StatusCodes.Status400BadRequest)
        //     .ProducesProblem(StatusCodes.Status401Unauthorized)
        //     .ProducesProblem(StatusCodes.Status500InternalServerError);

        return app;
    }
}