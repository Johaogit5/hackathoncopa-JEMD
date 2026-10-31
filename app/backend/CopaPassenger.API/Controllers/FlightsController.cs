using CopaPassenger.API.Models;
using CopaPassenger.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CopaPassenger.API.Controllers;

[ApiController]
[Route("api/flights")]
public class FlightsController(IMediator mediator) : ControllerBase
{
    [HttpGet("{flightNumber}")]
    public async Task<IActionResult> GetByFlightNumber(string flightNumber, CancellationToken ct)
    {
        var result = await mediator.Send(new GetFlightByNumberQuery(flightNumber), ct);
        return Ok(ApiResponse<object>.Ok(result));
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetByRouteAndDate(
        [FromQuery] string origin,
        [FromQuery] string destination,
        [FromQuery] DateOnly date,
        CancellationToken ct)
    {
        var result = await mediator.Send(new GetFlightsByRouteAndDateQuery(origin, destination, date), ct);
        return Ok(ApiResponse<object>.Ok(result));
    }
}
