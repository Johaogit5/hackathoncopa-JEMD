using CopaPassenger.Application.DTOs;
using CopaPassenger.Domain.Exceptions;
using CopaPassenger.Domain.Interfaces;
using MediatR;

namespace CopaPassenger.Application.Queries;

public record GetFlightByNumberQuery(string FlightNumber) : IRequest<FlightStatusDto>;

public class GetFlightByNumberQueryHandler(IFlightRepository repository)
    : IRequestHandler<GetFlightByNumberQuery, FlightStatusDto>
{
    public async Task<FlightStatusDto> Handle(GetFlightByNumberQuery request, CancellationToken ct)
    {
        var flight = await repository.GetByFlightNumberAsync(request.FlightNumber, ct)
            ?? throw new FlightNotFoundException(request.FlightNumber);

        return new FlightStatusDto(
            flight.Id,
            flight.FlightNumber,
            flight.Origin,
            flight.Destination,
            flight.Status,
            flight.Status.ToString(),
            flight.ScheduledDeparture,
            flight.EstimatedDeparture,
            flight.ScheduledArrival,
            flight.EstimatedArrival,
            flight.DepartureGate,
            flight.ArrivalGate,
            flight.DelayMinutes
        );
    }
}
