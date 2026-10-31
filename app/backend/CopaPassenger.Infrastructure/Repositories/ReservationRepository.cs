using CopaPassenger.Domain.Entities;
using CopaPassenger.Domain.Interfaces;
using CopaPassenger.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CopaPassenger.Infrastructure.Repositories;

public class ReservationRepository(CopaDbContext db) : IReservationRepository
{
    public async Task<Reservation?> GetByPnrAsync(string pnr, CancellationToken ct = default)
        => await db.Reservations
            .Include(r => r.AncillaryServices)
            .FirstOrDefaultAsync(r => r.PNR == pnr.ToUpperInvariant(), ct);

    public async Task<AncillaryService?> GetAncillaryServiceAsync(string pnr, Guid serviceId, CancellationToken ct = default)
    {
        var reservation = await GetByPnrAsync(pnr, ct);
        return reservation?.AncillaryServices.FirstOrDefault(s => s.Id == serviceId);
    }

    public async Task AddAncillaryServiceAsync(string pnr, Guid serviceId, CancellationToken ct = default)
    {
        var reservation = await db.Reservations
            .Include(r => r.AncillaryServices)
            .FirstOrDefaultAsync(r => r.PNR == pnr.ToUpperInvariant(), ct);

        var service = reservation?.AncillaryServices.FirstOrDefault(s => s.Id == serviceId);
        if (service is not null)
            service.IsAdded = true;
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
        => await db.SaveChangesAsync(ct);
}
