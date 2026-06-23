using MediatR;

namespace TattoStudio.Application.UseCases.Media.Commands.UploadMedia;

/// <summary>
/// Comando para subir un archivo multimedia a una cita.
/// </summary>
public record UploadMediaCommand(
    Guid   AppointmentId,
    Stream FileContent,
    string FileName,
    string ContentType,
    int    MediaType) : IRequest<UploadMediaResult>;

/// <summary>
/// Resultado devuelto tras subir un archivo multimedia.
/// </summary>
public record UploadMediaResult(Guid Id, string StorageUrl, int MediaType);
