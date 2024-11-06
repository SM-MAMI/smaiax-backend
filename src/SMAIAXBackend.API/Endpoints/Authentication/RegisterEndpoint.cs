using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Services.Interfaces;

namespace SMAIAXBackend.API.Endpoints.Authentication;

public static class RegisterEndpoint
{
    public static async Task<Ok<Guid>> Handle(
        IAuthenticationService authenticationService,
        [FromBody] RegisterDto registerDto)
    {
        var registeredUserId = await authenticationService.RegisterAsync(registerDto);
        return TypedResults.Ok(registeredUserId);
    }
}