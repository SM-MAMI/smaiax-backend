using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Services.Interfaces;

namespace SMAIAXBackend.API.Endpoints.Authentication;

public static class RefreshTokensEndpoint
{
    public static async Task<Results<Ok<TokenDto>, ProblemHttpResult>> Handle(
        IAuthenticationService authenticationService,
        [FromBody] TokenDto tokenDto)
    {
        var refreshedTokens = await authenticationService.RefreshTokensAsync(tokenDto);
        return TypedResults.Ok(refreshedTokens);
    }
}