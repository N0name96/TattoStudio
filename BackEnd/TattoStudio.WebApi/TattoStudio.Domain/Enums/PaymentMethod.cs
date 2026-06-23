namespace TattoStudio.Domain.Enums;

/// <summary>Método de pago utilizado en la transacción.</summary>
public enum PaymentMethod : short
{
    /// <summary>Pago en efectivo.</summary>
    Efectivo = 0,

    /// <summary>Pago con tarjeta bancaria.</summary>
    Tarjeta = 1,

    /// <summary>Pago mediante Bizum.</summary>
    Bizum = 2,

    /// <summary>Pago por transferencia bancaria.</summary>
    Transferencia = 3
}
