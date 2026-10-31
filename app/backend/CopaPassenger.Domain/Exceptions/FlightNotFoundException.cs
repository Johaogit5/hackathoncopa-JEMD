namespace CopaPassenger.Domain.Exceptions;

public class FlightNotFoundException(string flightNumber)
    : Exception($"No se encontró el vuelo '{flightNumber}'.");
