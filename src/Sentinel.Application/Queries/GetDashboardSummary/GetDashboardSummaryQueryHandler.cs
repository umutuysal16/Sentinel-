using MediatR;
using Microsoft.EntityFrameworkCore;
using Sentinel.Application.DTOs;
using Sentinel.Application.Interfaces;
using Sentinel.Domain.Enums;

namespace Sentinel.Application.Queries.GetDashboardSummary;

public class GetDashboardSummaryQueryHandler : IRequestHandler<GetDashboardSummaryQuery, DashboardSummaryDto>
{
    private readonly IApplicationDbContext _dbContext;

    public GetDashboardSummaryQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DashboardSummaryDto> Handle(GetDashboardSummaryQuery request, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var last24h = now.AddHours(-24);

        var totalAgents = await _dbContext.Agents.CountAsync(cancellationToken);
        var onlineAgents = await _dbContext.Agents
            .CountAsync(a => a.Status == AgentStatus.Online, cancellationToken);

        var totalLogs = await _dbContext.LogEntries.CountAsync(cancellationToken);
        var criticalLogs24h = await _dbContext.LogEntries
            .CountAsync(l => l.Level == Domain.Enums.LogLevel.Critical && l.Timestamp >= last24h, cancellationToken);

        var totalAlerts = await _dbContext.Alerts.CountAsync(cancellationToken);
        var unacknowledgedAlerts = await _dbContext.Alerts
            .CountAsync(a => !a.IsAcknowledged, cancellationToken);

        var averageRiskScore = await _dbContext.Alerts
            .Where(a => a.CreatedAt >= last24h)
            .Select(a => (double?)a.RiskScore)
            .AverageAsync(cancellationToken) ?? 0;

        var recentAlerts = await _dbContext.Alerts
            .Where(a => a.CreatedAt >= last24h)
            .ToListAsync(cancellationToken);

        var alertsByCategory = recentAlerts
            .GroupBy(a => a.Category.ToString())
            .ToDictionary(g => g.Key, g => g.Count());

        var recentLogs = await _dbContext.LogEntries
            .Where(l => l.Timestamp >= last24h)
            .Select(l => l.Timestamp)
            .ToListAsync(cancellationToken);

        var logVolumeByHour = recentLogs
            .GroupBy(t => new DateTime(t.Year, t.Month, t.Day, t.Hour, 0, 0, DateTimeKind.Utc))
            .Select(g => new LogVolumeByHourDto(g.Key, g.Count()))
            .OrderBy(x => x.Hour)
            .ToList();

        return new DashboardSummaryDto(
            totalAgents,
            onlineAgents,
            totalLogs,
            criticalLogs24h,
            totalAlerts,
            unacknowledgedAlerts,
            Math.Round(averageRiskScore, 1),
            alertsByCategory,
            logVolumeByHour);
    }
}
