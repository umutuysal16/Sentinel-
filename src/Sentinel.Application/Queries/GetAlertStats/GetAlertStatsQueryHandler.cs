using MediatR;
using Microsoft.EntityFrameworkCore;
using Sentinel.Application.DTOs;
using Sentinel.Application.Interfaces;

namespace Sentinel.Application.Queries.GetAlertStats;

public class GetAlertStatsQueryHandler : IRequestHandler<GetAlertStatsQuery, AlertStatsDto>
{
    private readonly IApplicationDbContext _dbContext;

    public GetAlertStatsQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AlertStatsDto> Handle(GetAlertStatsQuery request, CancellationToken cancellationToken)
    {
        var alerts = await _dbContext.Alerts.ToListAsync(cancellationToken);

        var totalAlerts = alerts.Count;
        var unacknowledgedCount = alerts.Count(a => !a.IsAcknowledged);
        var acknowledgedCount = alerts.Count(a => a.IsAcknowledged);
        var averageRiskScore = alerts.Count > 0 ? Math.Round(alerts.Average(a => a.RiskScore), 1) : 0;
        var highestRiskScore = alerts.Count > 0 ? alerts.Max(a => a.RiskScore) : 0;

        var byCategory = alerts
            .GroupBy(a => a.Category.ToString())
            .ToDictionary(g => g.Key, g => g.Count());

        var bySeverity = alerts
            .GroupBy(a => a.Severity.ToString())
            .ToDictionary(g => g.Key, g => g.Count());

        return new AlertStatsDto(
            totalAlerts,
            unacknowledgedCount,
            acknowledgedCount,
            averageRiskScore,
            highestRiskScore,
            byCategory,
            bySeverity);
    }
}
