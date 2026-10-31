using CopaPassenger.API.Models;
using CopaPassenger.Application.Commands;
using CopaPassenger.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CopaPassenger.API.Controllers;

[ApiController]
[Route("api/reservations")]
public class ReservationsController(IMediator mediator) : ControllerBase
{
    [HttpGet("{pnr}/ancillary-services")]
    public async Task<IActionResult> GetAncillaryServices(string pnr, CancellationToken ct)
    {
        var result = await mediator.Send(new GetAncillaryServicesQuery(pnr), ct);
        return Ok(ApiResponse<object>.Ok(result));
    }

    [HttpPost("{pnr}/ancillary-services/{serviceId:guid}")]
    public async Task<IActionResult> AddAncillaryService(string pnr, Guid serviceId, CancellationToken ct)
    {
        var result = await mediator.Send(new AddAncillaryServiceCommand(pnr, serviceId), ct);
        return StatusCode(StatusCodes.Status201Created, ApiResponse<object>.Ok(result));
    }
}
