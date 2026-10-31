using CopaPassenger.Application.Commands;
using FluentValidation;

namespace CopaPassenger.Application.Validators;

public class AddAncillaryServiceCommandValidator : AbstractValidator<AddAncillaryServiceCommand>
{
    public AddAncillaryServiceCommandValidator()
    {
        RuleFor(x => x.PNR).NotEmpty().WithMessage("El PNR no puede estar vacío.");
        RuleFor(x => x.ServiceId).NotEmpty().WithMessage("El serviceId no puede estar vacío.");
    }
}
