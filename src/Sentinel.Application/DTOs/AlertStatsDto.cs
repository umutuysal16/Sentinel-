namespace Sentinel.Application.DTOs;

public record AlertStatsDto(
    int TotalAlerts,
    int UnacknowledgedCount,
    int AcknowledgedCount,
    double AverageRiskScore,
    int HighestRiskScore,
    Dictionary<string, int> ByCategory,
    Dictionary<string, int> BySeverity);
