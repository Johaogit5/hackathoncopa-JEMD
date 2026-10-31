using CopaPassenger.Domain.Entities;
using CopaPassenger.Domain.Enums;
using CopaPassenger.Infrastructure.Persistence;
using CopaPassenger.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CopaPassenger.Tests;

public class FlightRepositoryTests
{
    private static CopaDbContext CreateContext()
    {
        var opts = new DbContextOptionsBuilder<CopaDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new CopaDbContext(opts);
    }

    [Fact]
    public async Task GetByFlightNumber_WhenExists_ReturnsFlight()
    {
        await using var db = CreateContext();
        db.Flights.Add(new Flight { Id = Guid.NewGuid(), FlightNumber = "CM201", Origin = "PTY", Destination = "BOG", Status = FlightStatus.Scheduled, ScheduledDeparture = DateTime.UtcNow, ScheduledArrival = DateTime.UtcNow.AddHours(3) });
        await db.SaveChangesAsync();

        var repo = new FlightRepository(db);
        var result = await repo.GetByFlightNumberAsync("cm201");

        Assert.NotNull(result);
        Assert.Equal("CM201", result!.FlightNumber);
    }

    [Fact]
    public async Task GetByFlightNumber_WhenNotExists_ReturnsNull()
    {
        await using var db = CreateContext();
        var repo = new FlightRepository(db);
        var result = await repo.GetByFlightNumberAsync("XX999");
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByRouteAndDate_ReturnsMatchingFlights()
    {
        await using var db = CreateContext();
        var dep = new DateTime(2026, 8, 12, 10, 0, 0, DateTimeKind.Utc);
        db.Flights.Add(new Flight { Id = Guid.NewGuid(), FlightNumber = "CM300", Origin = "PTY", Destination = "MIA", Status = FlightStatus.Scheduled, ScheduledDeparture = dep, ScheduledArrival = dep.AddHours(3) });
        await db.SaveChangesAsync();

        var repo = new FlightRepository(db);
        var results = await repo.GetByRouteAndDateAsync("PTY", "MIA", new DateOnly(2026, 8, 12));

        Assert.Single(results);
    }
}
