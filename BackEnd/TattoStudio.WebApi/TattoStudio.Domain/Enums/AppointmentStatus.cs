namespace TattoStudio.Domain.Enums;

/// <summary>
/// Estado del ciclo de vida de una cita en el estudio.
/// Se persiste como smallint en la base de datos (spec §3).
/// </summary>
public enum AppointmentStatus : short
{
    /// <summary>Cita reservada sin seña confirmada.</summary>
    Pendiente  = 0,

    /// <summary>Cita confirmada con seña recibida.</summary>
    Confirmada = 1,

    /// <summary>Sesión de tatuaje completada.</summary>
    Completada = 2,

    /// <summary>Cita cancelada por el cliente o el estudio.</summary>
    Cancelada  = 3
}
