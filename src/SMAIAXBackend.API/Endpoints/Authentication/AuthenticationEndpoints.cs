using Microsoft.AspNetCore.Http.HttpResults;

using SMAIAXBackend.Application.DTOs;

namespace SMAIAXBackend.API.Endpoints.Authentication;

public static class AuthenticationEndpoints
{
    public static WebApplication MapAuthenticationEndpoints(this WebApplication app)
    {
        const string contentType = "application/json";
        var group = app.MapGroup("/api/authentication")
            .WithTags("Authentication");

        group.MapPost("register", RegisterEndpoint.Handle)
            .WithName("register")
            .Accepts<RegisterDto>(contentType)
            .Produces<Guid>(StatusCodes.Status200OK, contentType)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapPost("login", LoginEndpoint.Handle)
            .WithName("login")
            .Accepts<LoginDto>(contentType)
            .Produces<TokenDto>(StatusCodes.Status200OK, contentType)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapPost("refresh", RefreshTokensEndpoint.Handle)
            .WithName("refresh")
            .Accepts<TokenDto>(contentType)
            .Produces<TokenDto>(StatusCodes.Status200OK, contentType)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapPost("logout", LogoutEndpoint.Handle)
            .WithName("logout")
            .Accepts<TokenDto>(contentType)
            .Produces<Ok>(StatusCodes.Status200OK, contentType)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        return app;
    }
}