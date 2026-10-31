namespace CopaPassenger.Domain.Entities;

public class Reservation
{
    public Guid Id { get; set; }
    public string PNR { get; set; } = string.Empty;
    public string PassengerName { get; set; } = string.Empty;
    public Guid FlightId { get; set; }
    public ICollection<AncillaryService> AncillaryServices { get; set; } = new List<AncillaryService>();
}
