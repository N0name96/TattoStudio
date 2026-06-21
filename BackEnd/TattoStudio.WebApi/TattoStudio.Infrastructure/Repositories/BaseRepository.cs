using TattoStudio.Infrastructure.Persistence;

namespace TattoStudio.Infrastructure.Repositories;

/// <summary>
/// Clase base genérica para repositorios EF Core. Provee <c>AddAsync</c> y <c>SaveChangesAsync</c>
/// comunes a todos los repositorios usando <c>DbContext.Set&lt;TEntity&gt;()</c>.
/// </summary>
public abstract class BaseRepository<TEntity> where TEntity : class
{
    /// <summary>DbContext compartido, accesible desde las subclases.</summary>
    protected readonly TattoStudioDbContext _context;

    /// <summary>Inyecta el DbContext principal.</summary>
    protected BaseRepository(TattoStudioDbContext context) => _context = context;

    /// <summary>Agrega la entidad al change tracker de EF Core.</summary>
    public Task AddAsync(TEntity entity, CancellationToken ct = default) =>
        _context.Set<TEntity>().AddAsync(entity, ct).AsTask();

    /// <summary>Persiste todos los cambios pendientes en la base de datos.</summary>
    public Task SaveChangesAsync(CancellationToken ct = default) =>
        _context.SaveChangesAsync(ct);
}
