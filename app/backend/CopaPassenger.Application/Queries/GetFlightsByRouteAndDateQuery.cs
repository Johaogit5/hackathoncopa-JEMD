using CopaPassenger.Application.DTOs;
using CopaPassenger.Domain.Interfaces;
using MediatR;

namespace CopaPassenger.Application.Queries;

public record GetFlightsByRouteAndDateQuery(string Origin, string Destination, DateOnly Date)
    : IRequest<IEnumerable<FlightStatusDto>>;

public class GetFlightsByRouteAndDateQueryHandler(IFlightRepository repository)
    : IRequestHandler<GetFlightsByRouteAndDateQuery, IEnumerable<FlightStatusDto>>
{
    public async Task<IEnumerable<FlightStatusDto>> Handle(GetFlightsByRouteAndDateQuery request, CancellationToken ct)
    {
        var flights = await repository.GetByRouteAndDateAsync(request.Origin, request.Destination, request.Date, ct);

        return flights.Select(f => new FlightStatusDto(
            f.Id,
            f.FlightNumber,
            f.Origin,
            f.Destination,
            f.Status,
            f.Status.ToString(),
            f.ScheduledDeparture,
            f.EstimatedDeparture,
            f.ScheduledArrival,
            f.EstimatedArrival,
            f.DepartureGate,
            f.ArrivalGate,
            f.DelayMinutes
        ));
    }
}
