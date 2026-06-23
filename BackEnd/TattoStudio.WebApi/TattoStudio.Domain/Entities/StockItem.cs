using TattoStudio.Domain.Exceptions;

namespace TattoStudio.Domain.Entities;

/// <summary>
/// Entidad de dominio que representa un material del inventario del estudio.
/// Aplica REG-05-02: cuando <c>CurrentQuantity &lt;= MinThreshold</c> se activa la alerta de reposición.
/// </summary>
public sealed class StockItem
{
    private StockItem() { }

    /// <summary>Crea un nuevo ítem de inventario con la cantidad y umbral indicados.</summary>
    public StockItem(string name, int currentQuantity, int minThreshold)
    {
        Id              = Guid.NewGuid();
        Name            = name;
        CurrentQuantity = currentQuantity;
        MinThreshold    = minThreshold;
    }

    /// <summary>Identificador único del ítem de stock.</summary>
    public Guid   Id              { get; private set; }

    /// <summary>Nombre del material (p.ej. "Tinta Negra", "Agujas 7M").</summary>
    public string Name            { get; private set; } = null!;

    /// <summary>Cantidad actual en inventario.</summary>
    public int    CurrentQuantity { get; private set; }

    /// <summary>Umbral mínimo por debajo del cual se genera una alerta de reposición.</summary>
    public int    MinThreshold    { get; private set; }

    /// <summary>
    /// Indica si el ítem necesita reposición (REG-05-02).
    /// Calculado: <c>CurrentQuantity &lt;= MinThreshold</c>.
    /// </summary>
    public bool   RequiresRestock => CurrentQuantity <= MinThreshold;

    /// <summary>
    /// Descuenta la cantidad indicada del inventario.
    /// Lanza <see cref="BusinessException"/> si no hay stock suficiente.
    /// </summary>
    public void Consume(int quantity)
    {
        if (quantity > CurrentQuantity)
            throw new BusinessException($"Stock insuficiente para '{Name}'.");
        CurrentQuantity -= quantity;
    }
}
