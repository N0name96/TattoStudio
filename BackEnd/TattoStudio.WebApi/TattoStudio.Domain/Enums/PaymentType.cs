namespace TattoStudio.Domain.Enums;

/// <summary>Tipo de pago: seña inicial o pago final de la sesión.</summary>
public enum PaymentType : short
{
    /// <summary>Seña o depósito inicial.</summary>
    Sena = 0,

    /// <summary>Pago final que cierra la deuda.</summary>
    PagoFinal = 1
}
