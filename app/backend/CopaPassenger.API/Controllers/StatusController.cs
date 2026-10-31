using CopaPassenger.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace CopaPassenger.API.Controllers;

[ApiController]
[Route("api")]
public class StatusController(
    IConfiguration configuration,
    ILogger<StatusController> logger) : ControllerBase
{
    [HttpGet("status")]
    public IActionResult GetStatus()
    {
        var enabled = configuration.GetValue<bool>("FeatureFlags:EnableFinalValidation");
        if (!enabled)
        {
            logger.LogWarning(
                "Status unavailable: final validation feature flag disabled. ErrorCode={ErrorCode}", "E5");
            return StatusCode(StatusCodes.Status503ServiceUnavailable,
                ApiResponse<object>.Fail("Error E5"));
        }

        return Ok(ApiResponse<object>.Ok(new { status = "Healthy", timestamp = DateTime.UtcNow }));
    }
}
