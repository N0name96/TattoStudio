namespace TattoStudio.Application.Contracts;

/// <summary>
/// Contrato del repositorio de métricas del Dashboard de administración (REG-07-03).
/// </summary>
public interface IDashboardRepository
{
    /// <summary>
    /// Devuelve las métricas consolidadas del mes y año indicados.
    /// </summary>
    Task<DashboardSummaryResult> GetSummaryAsync(int year, int month, CancellationToken ct = default);
}

/// <summary>
/// Resultado con las métricas del Dashboard para el periodo seleccionado.
/// </summary>
public record DashboardSummaryResult(
    decimal totalRevenue,
    int     totalAppointments,
    decimal totalCommissionsPaid,
    Guid?   topArtistId,
    int     lowStockAlertsCount);
