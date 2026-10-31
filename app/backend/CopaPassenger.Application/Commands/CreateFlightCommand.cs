using CopaPassenger.Application.DTOs;
using CopaPassenger.Domain.Entities;
using CopaPassenger.Domain.Enums;
using CopaPassenger.Domain.Interfaces;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace CopaPassenger.Application.Commands;

public record CreateFlightCommand(
    string FlightNumber,
    string Origin,
    string Destination,
    DateTime ScheduledDeparture,
    DateTime ScheduledArrival,
    string? DepartureGate,
    string? ArrivalGate
) : IRequest<FlightStatusDto>;

public class CreateFlightCommandHandler(IFlightRepository repository, IValidator<CreateFlightCommand> validator)
    : IRequestHandler<CreateFlightCommand, FlightStatusDto>
{
    public async Task<FlightStatusDto> Handle(CreateFlightCommand request, CancellationToken ct)
    {
        // No hay pipeline de validacion en MediatR, asi que se valida aqui.
        // ExceptionMiddleware convierte la ValidationException en HTTP 400.
        await validator.ValidateAndThrowAsync(request, ct);

        var flightNumber = request.FlightNumber.ToUpperInvariant();
        if (await repository.GetByFlightNumberAsync(flightNumber, ct) is not null)
            throw new ValidationException(
                [new ValidationFailure(nameof(request.FlightNumber), $"Ya existe el vuelo {flightNumber}.")]);

        var flight = new Flight
        {
            Id = Guid.NewGuid(),
            FlightNumber = flightNumber,
            Origin = request.Origin.ToUpperInvariant(),
            Destination = request.Destination.ToUpperInvariant(),
            Status = FlightStatus.Scheduled,
            ScheduledDeparture = request.ScheduledDeparture,
            EstimatedDeparture = request.ScheduledDeparture,
            ScheduledArrival = request.ScheduledArrival,
            EstimatedArrival = request.ScheduledArrival,
            DepartureGate = request.DepartureGate,
            ArrivalGate = request.ArrivalGate,
            DelayMinutes = 0
        };

        await repository.AddAsync(flight, ct);
        await repository.SaveChangesAsync(ct);

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
