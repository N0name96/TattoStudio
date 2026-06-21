using Microsoft.EntityFrameworkCore;
using TattoStudio.Application.Contracts;
using TattoStudio.Domain.Entities;
using TattoStudio.Infrastructure.Persistence;

namespace TattoStudio.Infrastructure.Repositories;

/// <summary>
/// Implementación EF Core del repositorio de consentimientos.
/// </summary>
public sealed class ConsentRepository : BaseRepository<Consent>, IConsentRepository
{
    /// <summary>Inyecta el DbContext principal.</summary>
    public ConsentRepository(TattoStudioDbContext context) : base(context) { }

    /// <inheritdoc/>
    public Task<Consent?> GetByAppointmentIdAsync(Guid appointmentId, CancellationToken ct = default) =>
        _context.Consents.FirstOrDefaultAsync(c => c.AppointmentId == appointmentId, ct);
}
