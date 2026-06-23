using TattoStudio.Domain.Entities;

namespace TattoStudio.Application.Contracts;

/// <summary>
/// Contrato de persistencia para la entidad <see cref="Invoice"/>.
/// </summary>
public interface IInvoiceRepository
{
    /// <summary>Persiste una nueva factura en el contexto (sin SaveChanges).</summary>
    Task AddAsync(Invoice invoice, CancellationToken ct = default);

    /// <summary>Devuelve la última factura emitida ordenada por fecha de emisión descendente.</summary>
    Task<Invoice?> FindLastAsync(CancellationToken ct = default);

    /// <summary>Busca una factura por su identificador único.</summary>
    Task<Invoice?> FindByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>Devuelve el número de facturas emitidas en el año indicado (para generar el número secuencial).</summary>
    Task<int> GetLastSequenceNumberAsync(int year, CancellationToken ct = default);

    /// <summary>Persiste los cambios pendientes en el contexto.</summary>
    Task SaveChangesAsync(CancellationToken ct = default);
}
