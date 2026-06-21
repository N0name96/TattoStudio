using Microsoft.EntityFrameworkCore;
using TattoStudio.Application.Contracts;
using TattoStudio.Domain.Entities;
using TattoStudio.Domain.Enums;
using TattoStudio.Infrastructure.Persistence;

namespace TattoStudio.Infrastructure.Repositories;

/// <summary>
/// Implementación EF Core del repositorio de citas.
/// La consulta de solapamiento (REG-03-02) usa <c>AnyAsync</c> tal como exige el spec §4.
/// </summary>
public sealed class AppointmentRepository : BaseRepository<Appointment>, IAppointmentRepository
{
    /// <summary>Inyecta el DbContext principal.</summary>
    public AppointmentRepository(TattoStudioDbContext context) : base(context) { }

    /// <inheritdoc/>
    public Task<Appointment?> FindByIdAsync(Guid id, CancellationToken ct = default) =>
        _context.Appointments.FirstOrDefaultAsync(a => a.Id == id, ct);

    /// <inheritdoc/>
    public Task<bool> HasOverlapAsync(
        Guid              artistId,
        DateTime          dateTime,
        int               durationHours,
        CancellationToken ct = default)
    {
        var newEnd = dateTime.AddHours(durationHours);

        return _context.Appointments.AnyAsync(a =>
            a.ArtistId == artistId &&
            (a.Status == AppointmentStatus.Pendiente ||
             a.Status == AppointmentStatus.Confirmada) &&
            a.DateTime            < newEnd &&
            a.DateTime.AddHours(a.DurationHours) > dateTime,
            ct);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<Appointment>> GetByDateRangeAsync(
        DateTime?         from      = null,
        DateTime?         to        = null,
        Guid?             clientId  = null,
        Guid?             artistId  = null,
        CancellationToken ct        = default)
    {
        var query = _context.Appointments.AsQueryable();

        if (from.HasValue)
            query = query.Where(a => a.DateTime >= from.Value);

        if (to.HasValue)
            query = query.Where(a => a.DateTime <= to.Value);

        if (clientId.HasValue)
            query = query.Where(a => a.ClientId == clientId.Value);

        if (artistId.HasValue)
            query = query.Where(a => a.ArtistId == artistId.Value);

        return await query.ToListAsync(ct);
    }

}
