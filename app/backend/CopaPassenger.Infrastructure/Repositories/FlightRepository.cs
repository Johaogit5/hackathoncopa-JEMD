using CopaPassenger.Domain.Entities;
using CopaPassenger.Domain.Interfaces;
using CopaPassenger.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CopaPassenger.Infrastructure.Repositories;

public class FlightRepository(CopaDbContext db) : IFlightRepository
{
    public async Task<Flight?> GetByFlightNumberAsync(string flightNumber, CancellationToken ct = default)
        => await db.Flights
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.FlightNumber == flightNumber.ToUpperInvariant(), ct);

    public async Task<IEnumerable<Flight>> GetByRouteAndDateAsync(string origin, string destination, DateOnly date, CancellationToken ct = default)
        => await db.Flights
            .AsNoTracking()
            .Where(f =>
                f.Origin == origin.ToUpperInvariant() &&
                f.Destination == destination.ToUpperInvariant() &&
                DateOnly.FromDateTime(f.ScheduledDeparture) == date)
            .OrderBy(f => f.ScheduledDeparture)
            .ToListAsync(ct);

    public async Task AddAsync(Flight flight, CancellationToken ct = default)
        => await db.Flights.AddAsync(flight, ct);

    public async Task SaveChangesAsync(CancellationToken ct = default)
        => await db.SaveChangesAsync(ct);
}
