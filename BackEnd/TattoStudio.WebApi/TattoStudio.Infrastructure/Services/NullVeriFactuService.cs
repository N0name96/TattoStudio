using TattoStudio.Application.Contracts;
using TattoStudio.Domain.Entities;
using TattoStudio.Domain.Enums;

namespace TattoStudio.Infrastructure.Services;

/// <summary>
/// Implementación no-op del servicio VeriFactu para entornos de desarrollo y tests.
/// Devuelve siempre <see cref="AeataStatus.PendienteEnvio"/> sin realizar ninguna llamada externa.
/// </summary>
public sealed class NullVeriFactuService : IVeriFactuService
{
    /// <summary>Simula el envío sin realizar ninguna llamada real a la AEAT.</summary>
    public Task<AeataStatus> SendInvoiceAsync(Invoice invoice, CancellationToken ct = default)
        => Task.FromResult(AeataStatus.PendienteEnvio);
}
