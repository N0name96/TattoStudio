using MediatR;
using TattoStudio.Application.Contracts;

namespace TattoStudio.Application.UseCases.Dashboard.Queries.GetDashboardSummary;

/// <summary>
/// Handler que devuelve el resumen de métricas del Dashboard (REG-07-03).
/// </summary>
public sealed class GetDashboardSummaryQueryHandler
    : IRequestHandler<GetDashboardSummaryQuery, DashboardSummaryResult>
{
    private readonly IDashboardRepository _dashboard;

    /// <summary>Inyecta el repositorio del Dashboard.</summary>
    public GetDashboardSummaryQueryHandler(IDashboardRepository dashboard)
    {
        _dashboard = dashboard;
    }

    /// <summary>Delega la consulta al repositorio y devuelve el resultado directamente.</summary>
    public Task<DashboardSummaryResult> Handle(
        GetDashboardSummaryQuery query,
        CancellationToken        ct)
    {
        return _dashboard.GetSummaryAsync(query.Year, query.Month, ct);
    }
}
