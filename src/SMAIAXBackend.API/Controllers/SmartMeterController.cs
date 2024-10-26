using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Services.Interfaces;

namespace SMAIAXBackend.API.Controllers;

[Route("api/smartMeters")]
[ApiController]
public class SmartMeterController(ISmartMeterCreateService smartMeterCreateService, ISmartMeterListService smartMeterListService) : ControllerBase
{
    [HttpGet(Name = "getSmartMeters")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize]
    public async Task<ActionResult<List<SmartMeterOverviewDto>>> GetSmartMeters()
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        var smartMeters = await smartMeterListService.GetSmartMetersByUserIdAsync(userIdClaim);

        return Ok(smartMeters);
    }

    [HttpPost(Name = "addSmartMeter")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize]
    public async Task<ActionResult<Guid>> AddSmartMeter([FromBody] SmartMeterCreateDto smartMeterCreateDto)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        var id = await smartMeterCreateService.AddSmartMeterAsync(smartMeterCreateDto, userIdClaim);

        // TODO: Change to CreatedAtRoute when GetById exists
        return Ok(id);
    }
}