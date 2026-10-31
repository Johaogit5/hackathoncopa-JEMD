namespace CopaPassenger.Domain.Exceptions;

public class DuplicateAncillaryServiceException(string pnr, string serviceId)
    : Exception($"El servicio '{serviceId}' ya fue agregado a la reserva '{pnr}'.");
