using CopaPassenger.Application.Commands;
using CopaPassenger.Application.Validators;
using CopaPassenger.Domain.Enums;
using CopaPassenger.Infrastructure.Persistence;
using CopaPassenger.Infrastructure.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace CopaPassenger.Tests;

public class CreateFlightCommandHandlerTests
{
    private static CopaDbContext CreateContext()
    {
        var opts = new DbContextOptionsBuilder<CopaDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new CopaDbContext(opts);
    }

    private static CreateFlightCommand BuildCommand(string flightNumber = "cm900", string origin = "pty", string destination = "bog")
    {
        var dep = new DateTime(2026, 11, 1, 10, 0, 0, DateTimeKind.Utc);
        return new CreateFlightCommand(flightNumber, origin, destination, dep, dep.AddHours(2), "G1", "A1");
    }

    [Fact]
    public async Task Handle_WhenValid_PersistsFlightAndReturnsDto()
    {
        await using var db = CreateContext();
        var repo = new FlightRepository(db);
        var handler = new CreateFlightCommandHandler(repo, new CreateFlightCommandValidator());

        var result = await handler.Handle(BuildCommand(), default);

        Assert.Equal("CM900", result.FlightNumber);
        Assert.Equal("PTY", result.Origin);
        Assert.Equal(FlightStatus.Scheduled, result.Status);
        Assert.NotNull(await repo.GetByFlightNumberAsync("CM900"));
        Assert.Single(await repo.GetByRouteAndDateAsync("PTY", "BOG", new DateOnly(2026, 11, 1)));
    }

    [Fact]
    public async Task Handle_WhenFlightNumberExists_ThrowsValidation()
    {
        await using var db = CreateContext();
        var handler = new CreateFlightCommandHandler(new FlightRepository(db), new CreateFlightCommandValidator());
        await handler.Handle(BuildCommand(), default);

        await Assert.ThrowsAsync<ValidationException>(() => handler.Handle(BuildCommand(), default));
    }

    [Fact]
    public async Task Handle_WhenInvalid_ThrowsValidation()
    {
        await using var db = CreateContext();
        var handler = new CreateFlightCommandHandler(new FlightRepository(db), new CreateFlightCommandValidator());

        await Assert.ThrowsAsync<ValidationException>(
            () => handler.Handle(BuildCommand(destination: "PTY"), default));
    }
}
