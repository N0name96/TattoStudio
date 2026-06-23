using MediatR;
using TattoStudio.Application.Contracts;

namespace TattoStudio.Application.UseCases.Dashboard.Queries.GetDashboardSummary;

/// <summary>
/// Consulta para obtener el resumen de métricas del Dashboard para el mes indicado.
/// </summary>
public record GetDashboardSummaryQuery(int Year, int Month) : IRequest<DashboardSummaryResult>;
