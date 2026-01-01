using Application.Delivery;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DeliveryController : ControllerBase
{
    private readonly DeliveryQuery _deliveryQuery;

    public DeliveryController(DeliveryQuery deliveryQuery)
    {
        _deliveryQuery = deliveryQuery;
    }

    [HttpGet("health")]
    public IActionResult HealthCheck()
    {
        var status = _deliveryQuery.Execute();
        return Ok(new
        {
            status = status.Status,
            timestamp = status.Timestamp,
            message = "Delivery service is healthy"
        });
    }

    [HttpGet("status")]
    public IActionResult GetStatus()
    {
        var status = _deliveryQuery.Execute();
        return Ok(status);
    }
}
