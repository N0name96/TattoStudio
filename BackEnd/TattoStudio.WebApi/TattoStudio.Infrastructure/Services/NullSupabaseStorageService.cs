using TattoStudio.Application.Contracts;

namespace TattoStudio.Infrastructure.Services;

/// <summary>
/// Implementación no-op del servicio de Supabase Storage para entornos de desarrollo y tests.
/// Devuelve una URL ficticia sin subir archivos reales.
/// </summary>
public sealed class NullSupabaseStorageService : ISupabaseStorageService
{
    /// <summary>Simula la subida del archivo y devuelve una URL ficticia.</summary>
    public Task<string> UploadAsync(
        string            bucket,
        string            fileName,
        Stream            content,
        string            contentType,
        CancellationToken ct = default)
        => Task.FromResult(
            $"https://fake.supabase.co/storage/v1/object/public/{bucket}/{fileName}");

    /// <summary>Simula la eliminación del archivo sin realizar ninguna operación real.</summary>
    public Task DeleteAsync(string bucket, string filePath, CancellationToken ct = default)
        => Task.CompletedTask;
}
