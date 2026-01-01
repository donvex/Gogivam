using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DeliveriesController : ControllerBase
{
    [HttpGet("check")]
    public IActionResult Check()
    {
        return Ok("ready!");
    }
}
