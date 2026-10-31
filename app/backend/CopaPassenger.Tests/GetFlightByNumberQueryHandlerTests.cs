using CopaPassenger.Application.Queries;
using CopaPassenger.Domain.Entities;
using CopaPassenger.Domain.Enums;
using CopaPassenger.Domain.Exceptions;
using CopaPassenger.Domain.Interfaces;
using NSubstitute;

namespace CopaPassenger.Tests;

public class GetFlightByNumberQueryHandlerTests
{
    private readonly IFlightRepository _repo = Substitute.For<IFlightRepository>();

    [Fact]
    public async Task Handle_WhenFlightExists_ReturnsDto()
    {
        var flight = new Flight
        {
            Id = Guid.NewGuid(),
            FlightNumber = "CM201",
            Origin = "PTY",
            Destination = "BOG",
            Status = FlightStatus.Scheduled,
            ScheduledDeparture = DateTime.UtcNow.AddHours(2),
            ScheduledArrival = DateTime.UtcNow.AddHours(5)
        };
        _repo.GetByFlightNumberAsync("CM201").Returns(flight);

        var handler = new GetFlightByNumberQueryHandler(_repo);
        var result = await handler.Handle(new GetFlightByNumberQuery("CM201"), default);

        Assert.Equal("CM201", result.FlightNumber);
        Assert.Equal("PTY", result.Origin);
    }

    [Fact]
    public async Task Handle_WhenFlightNotFound_ThrowsFlightNotFoundException()
    {
        _repo.GetByFlightNumberAsync("CM999").Returns((Flight?)null);

        var handler = new GetFlightByNumberQueryHandler(_repo);

        await Assert.ThrowsAsync<FlightNotFoundException>(
            () => handler.Handle(new GetFlightByNumberQuery("CM999"), default));
    }
}
