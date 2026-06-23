using AutoMapper;
using MediatR;
using TattoStudio.Application.Contracts;
using TattoStudio.Domain.Entities;
using TattoStudio.Domain.Exceptions;

namespace TattoStudio.Application.UseCases.Billing.Commands.IssueInvoice;

/// <summary>
/// Handler que emite una factura para el pago indicado, generando el número secuencial
/// y la cadena criptográfica SHA-256 para VeriFactu (REG-06-01).
/// </summary>
public sealed class IssueInvoiceCommandHandler
    : IRequestHandler<IssueInvoiceCommand, IssueInvoiceResult>
{
    private readonly IPaymentRepository  _payments;
    private readonly IInvoiceRepository  _invoices;
    private readonly IVeriFactuService   _veriFactu;
    private readonly IMapper             _mapper;

    /// <summary>Inyecta los repositorios, el servicio VeriFactu y el mapper.</summary>
    public IssueInvoiceCommandHandler(
        IPaymentRepository  payments,
        IInvoiceRepository  invoices,
        IVeriFactuService   veriFactu,
        IMapper             mapper)
    {
        _payments  = payments;
        _invoices  = invoices;
        _veriFactu = veriFactu;
        _mapper    = mapper;
    }

    /// <summary>
    /// Genera la factura con el número secuencial del año, calcula la base imponible
    /// (IVA 21%), encadena el hash SHA-256 de la factura anterior y envía a VeriFactu.
    /// </summary>
    public async Task<IssueInvoiceResult> Handle(
        IssueInvoiceCommand cmd,
        CancellationToken   ct)
    {
        var payment = await _payments.FindByIdAsync(cmd.PaymentId, ct)
            ?? throw new NotFoundException($"Pago con Id '{cmd.PaymentId}' no encontrado.");

        var amountBase = Math.Round(payment.Amount / 1.21m, 2);
        var taxAmount  = Math.Round(payment.Amount - amountBase, 2);

        var lastInvoice   = await _invoices.FindLastAsync(ct);
        var previousHash  = lastInvoice is null ? string.Empty : ComputeHash(lastInvoice);

        var year = DateTime.UtcNow.Year;
        var seq  = await _invoices.GetLastSequenceNumberAsync(year, ct) + 1;
        var invoiceNumber = $"TF-{year}-{seq:D4}";

        var invoice = new Invoice(cmd.PaymentId, invoiceNumber, amountBase, taxAmount, previousHash);

        await _invoices.AddAsync(invoice, ct);
        await _invoices.SaveChangesAsync(ct);

        // Envío asíncrono a VeriFactu; el fallo no bloquea al usuario (REG-06-02)
        _ = _veriFactu.SendInvoiceAsync(invoice, ct);

        return _mapper.Map<IssueInvoiceResult>(invoice);
    }

    /// <summary>Calcula el hash SHA-256 de los datos críticos de una factura.</summary>
    private static string ComputeHash(Invoice inv)
    {
        var raw = $"{inv.InvoiceNumber}{inv.AmountBase}{inv.TaxAmount}{inv.IssueDate:o}";
        return Convert.ToHexString(
            System.Security.Cryptography.SHA256.HashData(
                System.Text.Encoding.UTF8.GetBytes(raw))).ToLowerInvariant();
    }
}
