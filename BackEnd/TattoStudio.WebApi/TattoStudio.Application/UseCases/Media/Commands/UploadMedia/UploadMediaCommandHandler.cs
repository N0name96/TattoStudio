using AutoMapper;
using MediatR;
using TattoStudio.Application.Contracts;
using TattoStudio.Domain.Entities;
using TattoStudio.Domain.Exceptions;

namespace TattoStudio.Application.UseCases.Media.Commands.UploadMedia;

/// <summary>
/// Handler que sube un archivo multimedia a Supabase Storage y registra el media en la cita (REG-07-01).
/// </summary>
public sealed class UploadMediaCommandHandler
    : IRequestHandler<UploadMediaCommand, UploadMediaResult>
{
    private static readonly HashSet<string> AllowedTypes =
        new(StringComparer.OrdinalIgnoreCase) { "image/jpeg", "image/png", "image/webp" };

    private const long MaxFileSizeBytes = 5L * 1024 * 1024;

    private readonly IAppointmentRepository      _appointments;
    private readonly IAppointmentMediaRepository _mediaRepository;
    private readonly ISupabaseStorageService     _storage;
    private readonly IMapper                     _mapper;

    /// <summary>Inyecta los repositorios, el servicio de storage y el mapper.</summary>
    public UploadMediaCommandHandler(
        IAppointmentRepository      appointments,
        IAppointmentMediaRepository mediaRepository,
        ISupabaseStorageService     storage,
        IMapper                     mapper)
    {
        _appointments    = appointments;
        _mediaRepository = mediaRepository;
        _storage         = storage;
        _mapper          = mapper;
    }

    /// <summary>
    /// Valida formato y tamaño del archivo, lo sube a Supabase Storage
    /// y persiste el registro de multimedia en la cita.
    /// </summary>
    public async Task<UploadMediaResult> Handle(
        UploadMediaCommand cmd,
        CancellationToken  ct)
    {
        _ = await _appointments.FindByIdAsync(cmd.AppointmentId, ct)
            ?? throw new NotFoundException($"Cita con Id '{cmd.AppointmentId}' no encontrada.");

        if (!AllowedTypes.Contains(cmd.ContentType))
            throw new BusinessException(
                "El formato de archivo no es válido. Solo se permiten imagen/jpeg, imagen/png o imagen/webp.");

        if (cmd.FileContent.CanSeek && cmd.FileContent.Length > MaxFileSizeBytes)
            throw new BusinessException("El archivo supera el tamaño máximo de 5 MB.");

        var bucket   = cmd.MediaType == 0 ? "tattoo-references" : "tattoo-results";
        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(cmd.FileName)}";

        var storageUrl  = await _storage.UploadAsync(bucket, fileName, cmd.FileContent, cmd.ContentType, ct);
        var storagePath = $"{bucket}/{fileName}";

        var media = new AppointmentMedia(
            cmd.AppointmentId,
            storageUrl,
            storagePath,
            (Domain.Enums.MediaType)cmd.MediaType);

        await _mediaRepository.AddAsync(media, ct);
        await _mediaRepository.SaveChangesAsync(ct);

        return _mapper.Map<UploadMediaResult>(media);
    }
}
