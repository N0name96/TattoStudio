using Microsoft.EntityFrameworkCore;
using TattoStudio.Application.Contracts;
using TattoStudio.Domain.Entities;
using TattoStudio.Infrastructure.Persistence;

namespace TattoStudio.Infrastructure.Repositories;

/// <summary>
/// Implementación EF Core del repositorio de pagos.
/// </summary>
public sealed class PaymentRepository : BaseRepository<Payment>, IPaymentRepository
{
    /// <summary>Inyecta el DbContext principal.</summary>
    public PaymentRepository(TattoStudioDbContext context) : base(context) { }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<Payment>> GetByAppointmentIdAsync(Guid appointmentId, CancellationToken ct = default) =>
        await _context.Payments
            .Where(p => p.AppointmentId == appointmentId)
            .ToListAsync(ct);

    /// <inheritdoc/>
    public Task<Payment?> FindByIdAsync(Guid id, CancellationToken ct = default) =>
        _context.Payments.FirstOrDefaultAsync(p => p.Id == id, ct);
}
