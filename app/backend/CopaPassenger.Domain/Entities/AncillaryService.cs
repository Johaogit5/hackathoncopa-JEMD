using CopaPassenger.Domain.Enums;

namespace CopaPassenger.Domain.Entities;

public class AncillaryService
{
    public Guid Id { get; set; }
    public Guid ReservationId { get; set; }
    public AncillaryCategory Category { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal PriceUsd { get; set; }
    public bool IsAvailable { get; set; }
    public bool IsAdded { get; set; }
}
