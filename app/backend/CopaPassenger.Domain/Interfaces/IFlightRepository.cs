using CopaPassenger.Domain.Entities;

namespace CopaPassenger.Domain.Interfaces;

public interface IFlightRepository
{
    Task<Flight?> GetByFlightNumberAsync(string flightNumber, CancellationToken ct = default);
    Task<IEnumerable<Flight>> GetByRouteAndDateAsync(string origin, string destination, DateOnly date, CancellationToken ct = default);
}
