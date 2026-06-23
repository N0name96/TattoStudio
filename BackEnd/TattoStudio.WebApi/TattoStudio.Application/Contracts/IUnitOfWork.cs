namespace TattoStudio.Application.Contracts;

/// <summary>
/// Abstracción de unidad de trabajo para operaciones transaccionales (REG-05-01).
/// Permite agrupar múltiples cambios en un solo commit atómico.
/// </summary>
public interface IUnitOfWork : IAsyncDisposable
{
    /// <summary>Inicia una transacción de base de datos (no-op en InMemory).</summary>
    Task BeginTransactionAsync(CancellationToken ct = default);

    /// <summary>Persiste todos los cambios y confirma la transacción activa.</summary>
    Task CommitAsync(CancellationToken ct = default);

    /// <summary>Revierte la transacción activa si existe.</summary>
    Task RollbackAsync(CancellationToken ct = default);
}
