using Microsoft.EntityFrameworkCore;
using TattoStudio.Application.Contracts;
using TattoStudio.Domain.Entities;
using TattoStudio.Infrastructure.Persistence;

namespace TattoStudio.Infrastructure.Repositories;

/// <summary>
/// Implementación EF Core del repositorio de facturas.
/// </summary>
public sealed class InvoiceRepository : BaseRepository<Invoice>, IInvoiceRepository
{
    /// <summary>Inyecta el DbContext principal.</summary>
    public InvoiceRepository(TattoStudioDbContext context) : base(context) { }

    /// <inheritdoc/>
    public Task<Invoice?> FindLastAsync(CancellationToken ct = default) =>
        _context.Invoices.OrderByDescending(i => i.IssueDate).FirstOrDefaultAsync(ct);

    /// <inheritdoc/>
    public Task<Invoice?> FindByIdAsync(Guid id, CancellationToken ct = default) =>
        _context.Invoices.FirstOrDefaultAsync(i => i.Id == id, ct);

    /// <inheritdoc/>
    public Task<int> GetLastSequenceNumberAsync(int year, CancellationToken ct = default) =>
        _context.Invoices
            .Where(i => i.IssueDate.Year == year)
            .CountAsync(ct);
}
