using CopaPassenger.Application.Queries;
using FluentValidation;

namespace CopaPassenger.Application.Validators;

public class GetFlightByNumberQueryValidator : AbstractValidator<GetFlightByNumberQuery>
{
    public GetFlightByNumberQueryValidator()
    {
        RuleFor(x => x.FlightNumber)
            .NotEmpty()
            .Matches(@"^[A-Z]{2}\d{1,4}$")
            .WithMessage("El número de vuelo debe tener formato válido (ej: CM201).");
    }
}
