using CopaPassenger.Application.DTOs;
using CopaPassenger.Domain.Exceptions;
using CopaPassenger.Domain.Interfaces;
using MediatR;

namespace CopaPassenger.Application.Commands;

public record AddAncillaryServiceCommand(string PNR, Guid ServiceId) : IRequest<AncillaryServiceDto>;

public class AddAncillaryServiceCommandHandler(IReservationRepository repository)
    : IRequestHandler<AddAncillaryServiceCommand, AncillaryServiceDto>
{
    public async Task<AncillaryServiceDto> Handle(AddAncillaryServiceCommand request, CancellationToken ct)
    {
        var reservation = await repository.GetByPnrAsync(request.PNR, ct)
            ?? throw new ReservationNotFoundException(request.PNR);

        var service = await repository.GetAncillaryServiceAsync(request.PNR, request.ServiceId, ct)
            ?? throw new FlightNotFoundException(request.ServiceId.ToString());

        if (service.IsAdded)
            throw new DuplicateAncillaryServiceException(request.PNR, request.ServiceId.ToString());

        await repository.AddAncillaryServiceAsync(request.PNR, request.ServiceId, ct);
        await repository.SaveChangesAsync(ct);

        return new AncillaryServiceDto(
            service.Id,
            service.Category,
            service.Category.ToString(),
            service.Name,
            service.Description,
            service.PriceUsd,
            service.IsAvailable,
            true
        );
    }
}
