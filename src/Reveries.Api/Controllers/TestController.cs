using Microsoft.AspNetCore.Mvc;

namespace Reveries.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    [HttpGet("ping")]
    public IActionResult Ping() => Ok("pong");
}