using CopaPassenger.Domain.Enums;

namespace CopaPassenger.Application.DTOs;

public record FlightStatusDto(
    Guid Id,
    string FlightNumber,
    string Origin,
    string Destination,
    FlightStatus Status,
    string StatusLabel,
    DateTime ScheduledDeparture,
    DateTime? EstimatedDeparture,
    DateTime ScheduledArrival,
    DateTime? EstimatedArrival,
    string? DepartureGate,
    string? ArrivalGate,
    int DelayMinutes
);
