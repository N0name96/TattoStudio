using MediatR;

namespace TattoStudio.Application.UseCases.Billing.Queries.GetVeriFactuStatus;

/// <summary>
/// Consulta para obtener el estado VeriFactu de una factura existente.
/// </summary>
public record GetVeriFactuStatusQuery(Guid InvoiceId) : IRequest<GetVeriFactuStatusResult>;

/// <summary>
/// Resultado de la consulta del estado VeriFactu de una factura.
/// </summary>
public record GetVeriFactuStatusResult(
    Guid           Id,
    string         InvoiceNumber,
    int            AeataStatus,
    DateTimeOffset IssueDate,
    string         PreviousInvoiceHash);
