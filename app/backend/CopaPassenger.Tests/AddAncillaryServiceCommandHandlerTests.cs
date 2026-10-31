using CopaPassenger.Application.Commands;
using CopaPassenger.Domain.Entities;
using CopaPassenger.Domain.Enums;
using CopaPassenger.Domain.Exceptions;
using CopaPassenger.Domain.Interfaces;
using NSubstitute;

namespace CopaPassenger.Tests;

public class AddAncillaryServiceCommandHandlerTests
{
    private readonly IReservationRepository _repo = Substitute.For<IReservationRepository>();

    private static Reservation BuildReservation(Guid serviceId, bool isAdded = false)
    {
        var res = new Reservation { Id = Guid.NewGuid(), PNR = "ABC123", PassengerName = "Test" };
        res.AncillaryServices.Add(new AncillaryService
        {
            Id = serviceId,
            ReservationId = res.Id,
            Category = AncillaryCategory.ExtraBaggage,
            Name = "Maleta Extra",
            PriceUsd = 35m,
            IsAvailable = true,
            IsAdded = isAdded
        });
        return res;
    }

    [Fact]
    public async Task Handle_WhenValid_ReturnsUpdatedDto()
    {
        var serviceId = Guid.NewGuid();
        var res = BuildReservation(serviceId);
        _repo.GetByPnrAsync("ABC123").Returns(res);
        _repo.GetAncillaryServiceAsync("ABC123", serviceId).Returns(res.AncillaryServices.First());

        var handler = new AddAncillaryServiceCommandHandler(_repo);
        var result = await handler.Handle(new AddAncillaryServiceCommand("ABC123", serviceId), default);

        Assert.True(result.IsAdded);
    }

    [Fact]
    public async Task Handle_WhenServiceAlreadyAdded_ThrowsDuplicate()
    {
        var serviceId = Guid.NewGuid();
        var res = BuildReservation(serviceId, isAdded: true);
        _repo.GetByPnrAsync("ABC123").Returns(res);
        _repo.GetAncillaryServiceAsync("ABC123", serviceId).Returns(res.AncillaryServices.First());

        var handler = new AddAncillaryServiceCommandHandler(_repo);

        await Assert.ThrowsAsync<DuplicateAncillaryServiceException>(
            () => handler.Handle(new AddAncillaryServiceCommand("ABC123", serviceId), default));
    }

    [Fact]
    public async Task Handle_WhenReservationNotFound_ThrowsReservationNotFound()
    {
        _repo.GetByPnrAsync("ZZZ999").Returns((Reservation?)null);

        var handler = new AddAncillaryServiceCommandHandler(_repo);

        await Assert.ThrowsAsync<ReservationNotFoundException>(
            () => handler.Handle(new AddAncillaryServiceCommand("ZZZ999", Guid.NewGuid()), default));
    }
}
