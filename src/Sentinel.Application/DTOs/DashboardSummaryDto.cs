namespace Sentinel.Application.DTOs;

public record DashboardSummaryDto(
    int TotalAgents,
    int OnlineAgents,
    int TotalLogs,
    int CriticalLogs24h,
    int TotalAlerts,
    int UnacknowledgedAlerts,
    double AverageRiskScore,
    Dictionary<string, int> AlertsByCategory,
    List<LogVolumeByHourDto> LogVolumeByHour);

public record LogVolumeByHourDto(DateTime Hour, int Count);
