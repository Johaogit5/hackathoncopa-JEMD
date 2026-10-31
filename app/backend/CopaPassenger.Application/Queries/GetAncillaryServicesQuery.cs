using CopaPassenger.Application.DTOs;
using CopaPassenger.Domain.Exceptions;
using CopaPassenger.Domain.Interfaces;
using MediatR;

namespace CopaPassenger.Application.Queries;

public record GetAncillaryServicesQuery(string PNR) : IRequest<ReservationSummaryDto>;

public class GetAncillaryServicesQueryHandler(IReservationRepository repository)
    : IRequestHandler<GetAncillaryServicesQuery, ReservationSummaryDto>
{
    public async Task<ReservationSummaryDto> Handle(GetAncillaryServicesQuery request, CancellationToken ct)
    {
        var reservation = await repository.GetByPnrAsync(request.PNR, ct)
            ?? throw new ReservationNotFoundException(request.PNR);

        var services = reservation.AncillaryServices.Select(s => new AncillaryServiceDto(
            s.Id,
            s.Category,
            s.Category.ToString(),
            s.Name,
            s.Description,
            s.PriceUsd,
            s.IsAvailable,
            s.IsAdded
        ));

        return new ReservationSummaryDto(reservation.PNR, reservation.PassengerName, services);
    }
}
