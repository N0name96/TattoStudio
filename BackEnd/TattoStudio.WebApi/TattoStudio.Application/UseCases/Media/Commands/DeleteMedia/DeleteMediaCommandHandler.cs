using MediatR;
using TattoStudio.Application.Contracts;
using TattoStudio.Domain.Exceptions;

namespace TattoStudio.Application.UseCases.Media.Commands.DeleteMedia;

/// <summary>
/// Handler que elimina un archivo multimedia de Supabase Storage y borra su registro.
/// </summary>
public sealed class DeleteMediaCommandHandler : IRequestHandler<DeleteMediaCommand>
{
    private readonly IAppointmentMediaRepository _mediaRepository;
    private readonly ISupabaseStorageService     _storage;

    /// <summary>Inyecta el repositorio de multimedia y el servicio de storage.</summary>
    public DeleteMediaCommandHandler(
        IAppointmentMediaRepository mediaRepository,
        ISupabaseStorageService     storage)
    {
        _mediaRepository = mediaRepository;
        _storage         = storage;
    }

    /// <summary>
    /// Busca el registro de multimedia, elimina el archivo de Supabase Storage
    /// y borra el registro de la base de datos.
    /// </summary>
    public async Task Handle(DeleteMediaCommand cmd, CancellationToken ct)
    {
        var media = await _mediaRepository.FindByIdAsync(cmd.MediaId, ct)
            ?? throw new NotFoundException($"Media con Id '{cmd.MediaId}' no encontrado.");

        var parts  = media.StoragePath.Split('/', 2);
        var bucket = parts[0];
        var path   = parts.Length > 1 ? parts[1] : string.Empty;

        await _storage.DeleteAsync(bucket, path, ct);
        await _mediaRepository.RemoveAsync(media, ct);
        await _mediaRepository.SaveChangesAsync(ct);
    }
}
