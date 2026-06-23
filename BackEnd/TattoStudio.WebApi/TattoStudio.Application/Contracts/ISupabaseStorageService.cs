namespace TattoStudio.Application.Contracts;

/// <summary>
/// Contrato del servicio de almacenamiento de archivos en Supabase Storage.
/// </summary>
public interface ISupabaseStorageService
{
    /// <summary>
    /// Sube un archivo al bucket indicado y devuelve la URL pública del archivo.
    /// </summary>
    Task<string> UploadAsync(string bucket, string fileName, Stream content, string contentType, CancellationToken ct = default);

    /// <summary>
    /// Elimina el archivo en la ruta indicada del bucket especificado.
    /// </summary>
    Task DeleteAsync(string bucket, string filePath, CancellationToken ct = default);
}
