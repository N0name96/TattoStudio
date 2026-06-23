namespace TattoStudio.Domain.Entities;

/// <summary>
/// Registro de la comisión calculada para un artista al completarse una cita (REG-05-01).
/// Almacena el importe adeudado como <c>TotalPagos * CommissionPercentage / 100</c>.
/// </summary>
public sealed class ArtistCommissionsLog
{
    private ArtistCommissionsLog() { }

    /// <summary>Crea un nuevo registro de comisión para la cita y el artista indicados.</summary>
    public ArtistCommissionsLog(Guid artistId, Guid appointmentId, decimal amountOwed)
    {
        Id            = Guid.NewGuid();
        ArtistId      = artistId;
        AppointmentId = appointmentId;
        AmountOwed    = amountOwed;
        CreatedAt     = DateTimeOffset.UtcNow;
    }

    /// <summary>Identificador único del registro.</summary>
    public Guid           Id            { get; private set; }

    /// <summary>FK al artista que recibirá la comisión.</summary>
    public Guid           ArtistId      { get; private set; }

    /// <summary>FK a la cita que generó la comisión.</summary>
    public Guid           AppointmentId { get; private set; }

    /// <summary>Importe de la comisión calculado sobre el total de pagos.</summary>
    public decimal        AmountOwed    { get; private set; }

    /// <summary>Fecha y hora UTC de creación del registro.</summary>
    public DateTimeOffset CreatedAt     { get; private set; }
}
