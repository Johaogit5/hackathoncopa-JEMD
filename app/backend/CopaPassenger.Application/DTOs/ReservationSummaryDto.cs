namespace CopaPassenger.Application.DTOs;

public record ReservationSummaryDto(
    string PNR,
    string PassengerName,
    IEnumerable<AncillaryServiceDto> AncillaryServices
);
