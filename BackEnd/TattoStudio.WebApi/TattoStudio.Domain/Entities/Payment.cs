using TattoStudio.Domain.Enums;

namespace TattoStudio.Domain.Entities;

/// <summary>
/// Entidad de dominio que representa un cobro (seña o pago final) asociado a una cita.
/// </summary>
public sealed class Payment
{
    private Payment() { }

    /// <summary>Crea un nuevo pago para la cita indicada.</summary>
    public Payment(Guid appointmentId, decimal amount, PaymentType type, PaymentMethod method)
    {
        Id            = Guid.NewGuid();
        AppointmentId = appointmentId;
        Amount        = amount;
        Type          = type;
        Method        = method;
        CreatedAt     = DateTimeOffset.UtcNow;
    }

    /// <summary>Identificador único del pago.</summary>
    public Guid           Id            { get; private set; }

    /// <summary>FK a la cita a la que pertenece el pago.</summary>
    public Guid           AppointmentId { get; private set; }

    /// <summary>Importe del pago en euros.</summary>
    public decimal        Amount        { get; private set; }

    /// <summary>Tipo de pago: seña o pago final.</summary>
    public PaymentType    Type          { get; private set; }

    /// <summary>Método de pago empleado.</summary>
    public PaymentMethod  Method        { get; private set; }

    /// <summary>Fecha y hora UTC en que se registró el pago.</summary>
    public DateTimeOffset CreatedAt     { get; private set; }

    /// <summary>Propiedad de navegación EF Core hacia la cita.</summary>
    public Appointment?   Appointment   { get; private set; }
}
