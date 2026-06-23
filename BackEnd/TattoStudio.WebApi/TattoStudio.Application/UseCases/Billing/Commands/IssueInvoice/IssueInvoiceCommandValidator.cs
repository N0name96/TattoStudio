using FluentValidation;

namespace TattoStudio.Application.UseCases.Billing.Commands.IssueInvoice;

/// <summary>
/// Validador FluentValidation para el comando <see cref="IssueInvoiceCommand"/>.
/// </summary>
public sealed class IssueInvoiceCommandValidator : AbstractValidator<IssueInvoiceCommand>
{
    /// <summary>Define las reglas de validación del comando.</summary>
    public IssueInvoiceCommandValidator()
    {
        RuleFor(x => x.PaymentId).NotEmpty();
    }
}
