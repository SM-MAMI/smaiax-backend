using Microsoft.AspNetCore.Http.HttpResults;

using SMAIAXBackend.Application.DTOs;

namespace SMAIAXBackend.API.Endpoints.Authentication;

public static class AuthenticationEndpoints
{
    public static void MapAuthenticationEndpoints(this WebApplication app)
    {
        const string contentType = "application/json";
        var group = app.MapGroup("/api/authentication");

        group.MapPost("register", RegisterEndpoint.Handle)
            .WithName("register")
            .Accepts<RegisterDto>(contentType)
            .Produces<Guid>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
        
        group.MapPost("login", LoginEndpoint.Handle)
            .WithName("login")
            .Accepts<LoginDto>(contentType)
            .Produces<TokenDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
        
        group.MapPost("refresh", RefreshTokensEndpoint.Handle)
            .WithName("refresh")
            .Accepts<TokenDto>(contentType)
            .Produces<TokenDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
        
        group.MapPost("logout", LogoutEndpoint.Handle)
            .WithName("logout")
            .Accepts<TokenDto>(contentType)
            .Produces<Ok>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}