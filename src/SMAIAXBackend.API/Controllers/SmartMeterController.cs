using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Services.Interfaces;

namespace SMAIAXBackend.API.Controllers;

[Route("api/smartMeters")]
[ApiController]
public class SmartMeterController(ISmartMeterCreateService smartMeterCreateService) : ControllerBase
{
    [HttpPost(Name = "addSmartMeter")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize]
    public async Task<ActionResult<Guid>> AddSmartMeter([FromBody] SmartMeterCreateDto smartMeterCreateDto)
    {
        var id = await smartMeterCreateService.AddSmartMeterAsync(smartMeterCreateDto, User);

        // TODO: Change to CreatedAtRoute when GetById exists
        return Ok(id);
    }
}