using CopaPassenger.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CopaPassenger.Infrastructure.Persistence;

public class CopaDbContext(DbContextOptions<CopaDbContext> options) : DbContext(options)
{
    public DbSet<Flight> Flights => Set<Flight>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<AncillaryService> AncillaryServices => Set<AncillaryService>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Reservation>()
            .HasMany(r => r.AncillaryServices)
            .WithOne()
            .HasForeignKey(s => s.ReservationId);
    }
}
