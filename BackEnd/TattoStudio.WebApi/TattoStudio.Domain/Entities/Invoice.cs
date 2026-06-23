using TattoStudio.Domain.Enums;

namespace TattoStudio.Domain.Entities;

/// <summary>
/// Entidad que representa una factura emitida para un pago.
/// Almacena la cadena criptográfica SHA-256 para el Registro VeriFactu (REG-06-01).
/// </summary>
public sealed class Invoice
{
    private Invoice() { }

    /// <summary>Crea una nueva factura para el pago indicado.</summary>
    public Invoice(Guid paymentId, string invoiceNumber, decimal amountBase, decimal taxAmount, string previousInvoiceHash)
    {
        Id                  = Guid.NewGuid();
        PaymentId           = paymentId;
        InvoiceNumber       = invoiceNumber;
        AmountBase          = amountBase;
        TaxAmount           = taxAmount;
        PreviousInvoiceHash = previousInvoiceHash;
        IssueDate           = DateTimeOffset.UtcNow;
        AeataStatus         = AeataStatus.PendienteEnvio;
    }

    /// <summary>Identificador único de la factura.</summary>
    public Guid           Id                  { get; private set; }

    /// <summary>FK al pago origen de la factura.</summary>
    public Guid           PaymentId           { get; private set; }

    /// <summary>Número de factura en formato TF-{año}-{seq:D4}.</summary>
    public string         InvoiceNumber       { get; private set; } = null!;

    /// <summary>Base imponible (sin IVA).</summary>
    public decimal        AmountBase          { get; private set; }

    /// <summary>Importe del IVA (21%).</summary>
    public decimal        TaxAmount           { get; private set; }

    /// <summary>Fecha y hora UTC de emisión de la factura.</summary>
    public DateTimeOffset IssueDate           { get; private set; }

    /// <summary>Hash SHA-256 de los datos críticos de la factura anterior (cadena VeriFactu).</summary>
    public string         PreviousInvoiceHash { get; private set; } = string.Empty;

    /// <summary>Estado del envío a la AEAT vía VeriFactu.</summary>
    public AeataStatus    AeataStatus         { get; private set; }

    /// <summary>Propiedad de navegación EF Core hacia el pago.</summary>
    public Payment?       Payment             { get; private set; }
}
