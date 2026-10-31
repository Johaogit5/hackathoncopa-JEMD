using CopaPassenger.Domain.Enums;

namespace CopaPassenger.Application.DTOs;

public record AncillaryServiceDto(
    Guid ServiceId,
    AncillaryCategory Category,
    string CategoryLabel,
    string Name,
    string Description,
    decimal PriceUsd,
    bool IsAvailable,
    bool IsAdded
);
