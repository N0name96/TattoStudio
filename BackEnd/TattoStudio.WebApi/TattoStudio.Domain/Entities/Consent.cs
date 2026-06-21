namespace TattoStudio.Domain.Entities;

/// <summary>
/// Entidad de dominio que representa el consentimiento informado vinculado a una cita.
/// Implementa la inmutabilidad del consentimiento firmado (REG-04-02).
/// </summary>
public sealed class Consent
{
    /// <summary>Constructor vacío requerido por EF Core.</summary>
    private Consent() { }

    /// <summary>
    /// Crea un nuevo consentimiento sin firmar vinculado a una cita y un cliente.
    /// </summary>
    /// <param name="appointmentId">Identificador de la cita asociada.</param>
    /// <param name="clientId">Identificador del cliente que debe firmar.</param>
    public Consent(Guid appointmentId, Guid clientId)
    {
        Id            = Guid.NewGuid();
        AppointmentId = appointmentId;
        ClientId      = clientId;
        IsSigned      = false;
    }

    /// <summary>Identificador único del consentimiento.</summary>
    public Guid Id { get; private set; }

    /// <summary>FK única hacia la cita. Relación 1:1 con <see cref="Appointment"/>.</summary>
    public Guid AppointmentId { get; private set; }

    /// <summary>FK hacia el cliente que debe firmar el consentimiento.</summary>
    public Guid ClientId { get; private set; }

    /// <summary>Indica si el consentimiento ha sido firmado por el cliente.</summary>
    public bool IsSigned { get; private set; }

    /// <summary>Marca de tiempo UTC en que se firmó el consentimiento. Nulo si aún no está firmado.</summary>
    public DateTimeOffset? SignedAt { get; private set; }

    /// <summary>Firma del cliente codificada en Base64. Nulo si aún no está firmado.</summary>
    public string? SignatureBase64 { get; private set; }

    /// <summary>Propiedad de navegación EF Core hacia la cita asociada.</summary>
    public Appointment? Appointment { get; private set; }

    /// <summary>
    /// Registra la firma del cliente sobre el consentimiento.
    /// </summary>
    /// <param name="signatureBase64">Firma codificada en Base64.</param>
    public void Sign(string signatureBase64)
    {
        IsSigned        = true;
        SignedAt        = DateTimeOffset.UtcNow;
        SignatureBase64 = signatureBase64;
    }
}
