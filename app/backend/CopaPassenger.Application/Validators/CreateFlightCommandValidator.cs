using CopaPassenger.Application.Commands;
using FluentValidation;

namespace CopaPassenger.Application.Validators;

public class CreateFlightCommandValidator : AbstractValidator<CreateFlightCommand>
{
    public CreateFlightCommandValidator()
    {
        RuleFor(x => x.FlightNumber)
            .NotEmpty()
            .Matches(@"^[A-Za-z]{2}\d{1,4}$")
            .WithMessage("El número de vuelo debe tener formato válido (ej: CM201).");
        RuleFor(x => x.Origin)
            .NotEmpty()
            .Matches(@"^[A-Za-z]{3}$")
            .WithMessage("El origen debe ser un código IATA de 3 letras (ej: PTY).");
        RuleFor(x => x.Destination)
            .NotEmpty()
            .Matches(@"^[A-Za-z]{3}$")
            .WithMessage("El destino debe ser un código IATA de 3 letras (ej: BOG).");
        RuleFor(x => x.Destination)
            .Must((cmd, destination) => !string.Equals(cmd.Origin, destination, StringComparison.OrdinalIgnoreCase))
            .WithMessage("El destino debe ser distinto del origen.");
        RuleFor(x => x.ScheduledDeparture).NotEmpty().WithMessage("La salida programada es obligatoria.");
        RuleFor(x => x.ScheduledArrival)
            .GreaterThan(x => x.ScheduledDeparture)
            .WithMessage("La llegada programada debe ser posterior a la salida.");
    }
}
