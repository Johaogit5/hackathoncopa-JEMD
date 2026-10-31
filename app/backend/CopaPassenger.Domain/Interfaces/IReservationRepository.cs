using CopaPassenger.Domain.Entities;

namespace CopaPassenger.Domain.Interfaces;

public interface IReservationRepository
{
    Task<Reservation?> GetByPnrAsync(string pnr, CancellationToken ct = default);
    Task<AncillaryService?> GetAncillaryServiceAsync(string pnr, Guid serviceId, CancellationToken ct = default);
    Task AddAncillaryServiceAsync(string pnr, Guid serviceId, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
