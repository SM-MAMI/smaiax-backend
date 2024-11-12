using SMAIAXBackend.Application.DTOs;

namespace SMAIAXBackend.API.Endpoints.User;

public static class UserEndpoints
{
    public static WebApplication MapUserEndpoints(this WebApplication app)
    {
        const string contentType = "application/json";

        var group = app.MapGroup("/api/user")
            .WithTags("User");

        group.MapGet("/", GetUserEndpoint.Handle)
            .WithName("getUser")
            .Produces<UserDto>(StatusCodes.Status200OK, contentType)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        return app;
    }
}