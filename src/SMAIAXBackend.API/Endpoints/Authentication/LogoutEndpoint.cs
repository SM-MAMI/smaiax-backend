using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Services.Interfaces;

namespace SMAIAXBackend.API.Endpoints.Authentication;

public static class LogoutEndpoint
{
    public static async Task<NoContent> Handle(
        IAuthenticationService authenticationService,
        [FromBody] TokenDto tokenDto)
    {
        await authenticationService.LogoutAsync(tokenDto.RefreshToken);
        return TypedResults.NoContent();
    }
}