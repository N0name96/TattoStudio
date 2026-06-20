using TattoStudio.Domain.Enums;

namespace TattoStudio.Domain.Entities;

/// <summary>
/// Entidad de dominio que representa una cita en el estudio de tatuajes.
/// Aplica REG-03-03: si se recibe una seña, el estado se establece a Confirmada
/// y HasDeposit pasa a true en el momento de la construcción.
/// </summary>
public sealed class Appointment
{
    private Appointment() { }

    /// <summary>
    /// Crea una nueva cita aplicando la lógica de negocio REG-03-03.
    /// </summary>
    public Appointment(
        Guid     clientId,
        Guid     artistId,
        DateTime dateTime,
        int      durationHours,
        decimal  depositAmount)
    {
        Id            = Guid.NewGuid();
        ClientId      = clientId;
        ArtistId      = artistId;
        DateTime      = dateTime;
        DurationHours = durationHours;
        DepositAmount = depositAmount;

        if (depositAmount > 0)
        {
            HasDeposit = true;
            Status     = AppointmentStatus.Confirmada;
        }
        else
        {
            HasDeposit = false;
            Status     = AppointmentStatus.Pendiente;
        }
    }

    /// <summary>Identificador único de la cita.</summary>
    public Guid              Id            { get; private set; }

    /// <summary>FK al cliente que reserva la cita.</summary>
    public Guid              ClientId      { get; private set; }

    /// <summary>FK al artista asignado.</summary>
    public Guid              ArtistId      { get; private set; }

    /// <summary>Fecha y hora de inicio de la sesión en UTC.</summary>
    public DateTime          DateTime      { get; private set; }

    /// <summary>Duración estimada de la sesión en horas.</summary>
    public int               DurationHours { get; private set; }

    /// <summary>Estado actual de la cita.</summary>
    public AppointmentStatus Status        { get; private set; }

    /// <summary>Indica si se ha recibido una seña.</summary>
    public bool              HasDeposit    { get; private set; }

    /// <summary>Importe de la seña en euros. Cero si no hay seña.</summary>
    public decimal           DepositAmount { get; private set; }

    /// <summary>Propiedad de navegación EF Core hacia el cliente.</summary>
    public Client?           Client        { get; private set; }

    /// <summary>Propiedad de navegación EF Core hacia el artista.</summary>
    public Artist?           Artist        { get; private set; }

    /// <summary>
    /// Registra el pago de una seña y confirma la cita (REG-03-03 post-creación).
    /// </summary>
    public void ConfirmDeposit(decimal depositAmount)
    {
        DepositAmount = depositAmount;
        HasDeposit    = true;
        Status        = AppointmentStatus.Confirmada;
    }
}
