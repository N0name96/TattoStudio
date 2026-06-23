using Microsoft.EntityFrameworkCore;
using TattoStudio.Application.Contracts;
using TattoStudio.Domain.Entities;
using TattoStudio.Infrastructure.Persistence;

namespace TattoStudio.Infrastructure.Repositories;

/// <summary>
/// Implementación EF Core del repositorio de multimedia de citas.
/// </summary>
public sealed class AppointmentMediaRepository : BaseRepository<AppointmentMedia>, IAppointmentMediaRepository
{
    /// <summary>Inyecta el DbContext principal.</summary>
    public AppointmentMediaRepository(TattoStudioDbContext context) : base(context) { }

    /// <inheritdoc/>
    public Task<AppointmentMedia?> FindByIdAsync(Guid id, CancellationToken ct = default) =>
        _context.AppointmentMedias.FirstOrDefaultAsync(m => m.Id == id, ct);

    /// <inheritdoc/>
    public Task RemoveAsync(AppointmentMedia media, CancellationToken ct = default)
    {
        _context.AppointmentMedias.Remove(media);
        return Task.CompletedTask;
    }
}
