using TattoStudio.Application.Contracts;
using TattoStudio.Domain.Entities;
using TattoStudio.Infrastructure.Persistence;

namespace TattoStudio.Infrastructure.Repositories;

/// <summary>
/// Implementación EF Core del repositorio de registros de comisiones de artistas.
/// </summary>
public sealed class ArtistCommissionsLogRepository : BaseRepository<ArtistCommissionsLog>, IArtistCommissionsLogRepository
{
    /// <summary>Inyecta el DbContext principal.</summary>
    public ArtistCommissionsLogRepository(TattoStudioDbContext context) : base(context) { }
}
