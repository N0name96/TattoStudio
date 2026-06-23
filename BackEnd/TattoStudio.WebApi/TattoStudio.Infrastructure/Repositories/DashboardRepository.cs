using Microsoft.EntityFrameworkCore;
using TattoStudio.Application.Contracts;
using TattoStudio.Infrastructure.Persistence;

namespace TattoStudio.Infrastructure.Repositories;

/// <summary>
/// Implementación EF Core del repositorio del Dashboard. Usa AsNoTracking y agrupaciones
/// LINQ ejecutadas directamente en el servidor de base de datos (REG-07-03).
/// </summary>
public sealed class DashboardRepository : IDashboardRepository
{
    private readonly TattoStudioDbContext _context;

    /// <summary>Inyecta el DbContext principal.</summary>
    public DashboardRepository(TattoStudioDbContext context) => _context = context;

    /// <inheritdoc/>
    public async Task<DashboardSummaryResult> GetSummaryAsync(int year, int month, CancellationToken ct = default)
    {
        var startDate = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
        var endDate   = startDate.AddMonths(1);

        var appointmentIdsInMonth = await _context.Appointments
            .AsNoTracking()
            .Where(a => a.DateTime >= startDate && a.DateTime < endDate)
            .Select(a => a.Id)
            .ToListAsync(ct);

        var totalAppointments = appointmentIdsInMonth.Count;

        var totalRevenue = await _context.Payments
            .AsNoTracking()
            .Where(p => appointmentIdsInMonth.Contains(p.AppointmentId))
            .SumAsync(p => (decimal?)p.Amount, ct) ?? 0m;

        var totalCommissionsPaid = await _context.ArtistCommissionsLogs
            .AsNoTracking()
            .Where(c => appointmentIdsInMonth.Contains(c.AppointmentId))
            .SumAsync(c => (decimal?)c.AmountOwed, ct) ?? 0m;

        var topArtistId = await _context.Appointments
            .AsNoTracking()
            .Where(a => appointmentIdsInMonth.Contains(a.Id))
            .GroupBy(a => a.ArtistId)
            .OrderByDescending(g => g.Count())
            .Select(g => (Guid?)g.Key)
            .FirstOrDefaultAsync(ct);

        var lowStockAlertsCount = await _context.StockItems
            .AsNoTracking()
            .CountAsync(s => s.CurrentQuantity <= s.MinThreshold, ct);

        return new DashboardSummaryResult(
            totalRevenue,
            totalAppointments,
            totalCommissionsPaid,
            topArtistId,
            lowStockAlertsCount);
    }
}
