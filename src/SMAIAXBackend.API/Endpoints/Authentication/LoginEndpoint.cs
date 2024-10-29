using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Services.Interfaces;

namespace SMAIAXBackend.API.Endpoints.Authentication;

public static class LoginEndpoint
{
    public static async Task<Results<Ok<TokenDto>, ProblemHttpResult>> Handle(
        IAuthenticationService authenticationService,
        [FromBody] LoginDto loginDto)
    {
        var tokenDto = await authenticationService.LoginAsync(loginDto);
        return TypedResults.Ok(tokenDto);
    }
}