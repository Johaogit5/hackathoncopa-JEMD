using CopaPassenger.Domain.Entities;
using CopaPassenger.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace CopaPassenger.Infrastructure.Persistence;

public static class DbInitializer
{
    public static void SeedDatabase(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CopaDbContext>();

        if (db.Flights.Any()) return;

        var now = DateTime.UtcNow;

        // 5 rutas, 4 vuelos por ruta = 20 vuelos
        var routes = new[]
        {
            ("PTY", "BOG"), ("PTY", "MIA"), ("PTY", "GRU"), ("PTY", "MEX"), ("PTY", "SCL")
        };

        var statuses = new[] { FlightStatus.Scheduled, FlightStatus.Boarding, FlightStatus.Delayed, FlightStatus.InFlight };
        var flights = new List<Flight>();

        int flightNum = 201;
        foreach (var (origin, dest) in routes)
        {
            for (int i = 0; i < 4; i++)
            {
                var dep = now.AddHours(i * 2 + 1);
                var status = statuses[i % statuses.Length];
                var delay = status == FlightStatus.Delayed ? 45 : 0;
                flights.Add(new Flight
                {
                    Id = Guid.NewGuid(),
                    FlightNumber = $"CM{flightNum++}",
                    Origin = origin,
                    Destination = dest,
                    Status = status,
                    ScheduledDeparture = dep,
                    EstimatedDeparture = status == FlightStatus.Delayed ? dep.AddMinutes(delay) : dep,
                    ScheduledArrival = dep.AddHours(3),
                    EstimatedArrival = status == FlightStatus.Delayed ? dep.AddHours(3).AddMinutes(delay) : dep.AddHours(3),
                    DepartureGate = $"G{10 + i}",
                    ArrivalGate = $"A{5 + i}",
                    DelayMinutes = delay
                });
            }
        }

        db.Flights.AddRange(flights);

        // 3 reservas con servicios complementarios
        var reservations = new[]
        {
            new Reservation { Id = Guid.NewGuid(), PNR = "ABC123", PassengerName = "Juan Pérez", FlightId = flights[0].Id },
            new Reservation { Id = Guid.NewGuid(), PNR = "DEF456", PassengerName = "María García", FlightId = flights[4].Id },
            new Reservation { Id = Guid.NewGuid(), PNR = "GHI789", PassengerName = "Carlos López", FlightId = flights[8].Id }
        };

        var ancillaryServices = new List<AncillaryService>();
        foreach (var res in reservations)
        {
            ancillaryServices.AddRange(new[]
            {
                // ExtraBaggage x2
                new AncillaryService { Id = Guid.NewGuid(), ReservationId = res.Id, Category = AncillaryCategory.ExtraBaggage, Name = "Maleta Extra 23kg", Description = "Una maleta adicional de hasta 23 kg", PriceUsd = 35m, IsAvailable = true, IsAdded = false },
                new AncillaryService { Id = Guid.NewGuid(), ReservationId = res.Id, Category = AncillaryCategory.ExtraBaggage, Name = "Maleta Extra 32kg", Description = "Una maleta adicional de hasta 32 kg", PriceUsd = 60m, IsAvailable = true, IsAdded = false },
                // SeatSelection x2
                new AncillaryService { Id = Guid.NewGuid(), ReservationId = res.Id, Category = AncillaryCategory.SeatSelection, Name = "Asiento Preferencial", Description = "Asiento con más espacio para las piernas", PriceUsd = 25m, IsAvailable = true, IsAdded = false },
                new AncillaryService { Id = Guid.NewGuid(), ReservationId = res.Id, Category = AncillaryCategory.SeatSelection, Name = "Asiento Ventana", Description = "Asiento junto a la ventana", PriceUsd = 15m, IsAvailable = true, IsAdded = false },
                // SpecialMeals x2
                new AncillaryService { Id = Guid.NewGuid(), ReservationId = res.Id, Category = AncillaryCategory.SpecialMeals, Name = "Menú Vegetariano", Description = "Menú sin carne, con vegetales frescos", PriceUsd = 18m, IsAvailable = true, IsAdded = false },
                new AncillaryService { Id = Guid.NewGuid(), ReservationId = res.Id, Category = AncillaryCategory.SpecialMeals, Name = "Menú Sin Gluten", Description = "Menú libre de gluten", PriceUsd = 20m, IsAvailable = false, IsAdded = false }
            });
        }

        db.Reservations.AddRange(reservations);
        db.AncillaryServices.AddRange(ancillaryServices);
        db.SaveChanges();
    }
}
