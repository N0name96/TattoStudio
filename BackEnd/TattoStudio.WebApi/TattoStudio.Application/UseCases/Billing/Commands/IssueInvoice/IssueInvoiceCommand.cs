using MediatR;

namespace TattoStudio.Application.UseCases.Billing.Commands.IssueInvoice;

/// <summary>
/// Comando para emitir una nueva factura a partir de un pago existente.
/// </summary>
public record IssueInvoiceCommand(Guid PaymentId) : IRequest<IssueInvoiceResult>;

/// <summary>
/// Resultado devuelto tras emitir una factura.
/// </summary>
public record IssueInvoiceResult(
    Guid   Id,
    string InvoiceNumber,
    decimal AmountBase,
    decimal TaxAmount,
    string PreviousInvoiceHash,
    int    AeataStatus);
