namespace CopaPassenger.Domain.Exceptions;

public class ReservationNotFoundException(string pnr)
    : Exception($"No se encontró la reserva con PNR '{pnr}'.");
