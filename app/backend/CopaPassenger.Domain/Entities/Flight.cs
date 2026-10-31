using CopaPassenger.Domain.Enums;

namespace CopaPassenger.Domain.Entities;

public class Flight
{
    public Guid Id { get; set; }
    public string FlightNumber { get; set; } = string.Empty;
    public string Origin { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public FlightStatus Status { get; set; }
    public DateTime ScheduledDeparture { get; set; }
    public DateTime? EstimatedDeparture { get; set; }
    public DateTime ScheduledArrival { get; set; }
    public DateTime? EstimatedArrival { get; set; }
    public string? DepartureGate { get; set; }
    public string? ArrivalGate { get; set; }
    public int DelayMinutes { get; set; }
}
