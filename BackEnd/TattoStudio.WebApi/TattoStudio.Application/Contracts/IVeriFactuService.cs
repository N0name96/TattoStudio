using TattoStudio.Domain.Entities;
using TattoStudio.Domain.Enums;

namespace TattoStudio.Application.Contracts;

/// <summary>
/// Contrato del servicio de envío de facturas al sistema VeriFactu de la AEAT.
/// </summary>
public interface IVeriFactuService
{
    /// <summary>
    /// Envía la factura al sistema VeriFactu y devuelve el estado resultante del envío.
    /// </summary>
    Task<AeataStatus> SendInvoiceAsync(Invoice invoice, CancellationToken ct = default);
}
