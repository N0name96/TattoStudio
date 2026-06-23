using TattoStudio.Domain.Enums;

namespace TattoStudio.Domain.Entities;

/// <summary>
/// Entidad que representa un archivo multimedia asociado a una cita (REG-07-01).
/// </summary>
public sealed class AppointmentMedia
{
    private AppointmentMedia() { }

    /// <summary>Crea un nuevo registro de multimedia para la cita indicada.</summary>
    public AppointmentMedia(Guid appointmentId, string storageUrl, string storagePath, MediaType mediaType)
    {
        Id            = Guid.NewGuid();
        AppointmentId = appointmentId;
        StorageUrl    = storageUrl;
        StoragePath   = storagePath;
        MediaType     = mediaType;
        CreatedAt     = DateTimeOffset.UtcNow;
    }

    /// <summary>Identificador único del archivo multimedia.</summary>
    public Guid           Id            { get; private set; }

    /// <summary>FK a la cita a la que pertenece el archivo.</summary>
    public Guid           AppointmentId { get; private set; }

    /// <summary>URL pública del archivo en Supabase Storage.</summary>
    public string         StorageUrl    { get; private set; } = null!;

    /// <summary>Ruta relativa del archivo en Supabase Storage (bucket/fileName).</summary>
    public string         StoragePath   { get; private set; } = null!;

    /// <summary>Tipo de archivo multimedia.</summary>
    public MediaType      MediaType     { get; private set; }

    /// <summary>Fecha y hora UTC de subida del archivo.</summary>
    public DateTimeOffset CreatedAt     { get; private set; }

    /// <summary>Propiedad de navegación EF Core hacia la cita.</summary>
    public Appointment?   Appointment   { get; private set; }
}
