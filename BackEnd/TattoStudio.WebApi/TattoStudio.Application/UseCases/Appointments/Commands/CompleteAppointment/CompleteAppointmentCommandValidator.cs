using FluentValidation;

namespace TattoStudio.Application.UseCases.Appointments.Commands.CompleteAppointment;

/// <summary>Validador FluentValidation para <see cref="CompleteAppointmentCommand"/>.</summary>
public class CompleteAppointmentCommandValidator : AbstractValidator<CompleteAppointmentCommand>
{
    /// <summary>Define las reglas de validación del comando.</summary>
    public CompleteAppointmentCommandValidator()
    {
        RuleFor(x => x.StockItems).NotNull();
    }
}
