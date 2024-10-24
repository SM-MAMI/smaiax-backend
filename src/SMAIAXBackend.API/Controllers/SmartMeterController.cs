using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SMAIAXBackend.API.Controllers;

[Route("api/smartMeters")]
[ApiController]
public class SmartMeterController : ControllerBase
{
    [HttpGet]
    [Authorize]
    public string HelloWorld() => "Hello World!";
}