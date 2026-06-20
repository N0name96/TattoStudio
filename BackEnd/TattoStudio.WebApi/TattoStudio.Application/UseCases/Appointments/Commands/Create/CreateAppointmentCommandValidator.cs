using FluentValidation;

namespace TattoStudio.Application.UseCases.Appointments.Commands.Create;

/// <summary>
/// Validador FluentValidation para <see cref="CreateAppointmentCommand"/>.
/// Aplica REG-03-01: la fecha debe estar en el futuro (HTTP 400).
/// </summary>
public sealed class CreateAppointmentCommandValidator
    : AbstractValidator<CreateAppointmentCommand>
{
    public CreateAppointmentCommandValidator()
    {
        RuleFor(x => x.ClientId)
            .NotEmpty().WithMessage("El ClientId es obligatorio.");

        RuleFor(x => x.ArtistId)
            .NotEmpty().WithMessage("El ArtistId es obligatorio.");

        RuleFor(x => x.DateTime)
            .Must(dt => dt.ToUniversalTime() > DateTime.UtcNow)
            .WithMessage("La fecha de la cita debe estar en el futuro.");

        RuleFor(x => x.DurationHours)
            .GreaterThan(0)
            .WithMessage("La duración debe ser al menos 1 hora.");

        RuleFor(x => x.DepositAmount)
            .GreaterThanOrEqualTo(0m)
            .WithMessage("El importe de la seña no puede ser negativo.");
    }
}
