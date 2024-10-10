using Microsoft.AspNetCore.Mvc;
using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Services.Interfaces;

namespace SMAIAXBackend.API.Controllers;

[Route("api/authentication")]
[ApiController]
public class AuthenticationController(IAuthenticationService authenticationService) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes("application/json")]
    public async Task<ActionResult<Guid>> Register([FromBody] RegisterDto registerDto)
    {
        var registeredUserId = await authenticationService.RegisterAsync(registerDto);
        return Ok(registeredUserId);
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes("application/json")]
    public async Task<ActionResult<TokenDto>> Login([FromBody] LoginDto loginDto)
    {
        var tokenDto = await authenticationService.LoginAsync(loginDto);
        return Ok(tokenDto);
    }

    [HttpPost("refresh")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes("application/json")]
    public async Task<ActionResult<TokenDto>> RefreshTokens([FromBody] TokenDto tokenDto)
    {
        var refreshedTokens = await authenticationService.RefreshTokensAsync(tokenDto);
        return Ok(refreshedTokens);
    }

    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes("application/json")]
    public async Task<IActionResult> Logout([FromBody] TokenDto tokenDto)
    {
        await authenticationService.LogoutAsync(tokenDto.RefreshToken);
        return Ok();
    }
}