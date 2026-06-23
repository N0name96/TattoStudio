namespace TattoStudio.Domain.Enums;

/// <summary>
/// Estado del envío de la factura a la Agencia Estatal de Administración Tributaria (AEAT)
/// a través del sistema VeriFactu.
/// </summary>
public enum AeataStatus : short
{
    /// <summary>Pendiente de envío al servicio VeriFactu.</summary>
    PendienteEnvio = 0,

    /// <summary>Factura aceptada correctamente por la AEAT.</summary>
    Aceptada = 1,

    /// <summary>Factura rechazada por la AEAT.</summary>
    Rechazada = 2
}
