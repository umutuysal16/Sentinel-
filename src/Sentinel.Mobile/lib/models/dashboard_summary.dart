class DashboardSummary {
  final int totalAgents;
  final int onlineAgents;
  final int totalLogs;
  final int criticalLogs24h;
  final int totalAlerts;
  final int unacknowledgedAlerts;
  final double averageRiskScore;
  final Map<String, int> alertsByCategory;
  final List<LogVolumeByHour> logVolumeByHour;

  DashboardSummary({
    required this.totalAgents,
    required this.onlineAgents,
    required this.totalLogs,
    required this.criticalLogs24h,
    required this.totalAlerts,
    required this.unacknowledgedAlerts,
    required this.averageRiskScore,
    required this.alertsByCategory,
    required this.logVolumeByHour,
  });

  factory DashboardSummary.fromJson(Map<String, dynamic> json) {
    return DashboardSummary(
      totalAgents: json['totalAgents'] ?? 0,
      onlineAgents: json['onlineAgents'] ?? 0,
      totalLogs: json['totalLogs'] ?? 0,
      criticalLogs24h: json['criticalLogs24h'] ?? 0,
      totalAlerts: json['totalAlerts'] ?? 0,
      unacknowledgedAlerts: json['unacknowledgedAlerts'] ?? 0,
      averageRiskScore: (json['averageRiskScore'] ?? 0).toDouble(),
      alertsByCategory: Map<String, int>.from(json['alertsByCategory'] ?? {}),
      logVolumeByHour: (json['logVolumeByHour'] as List? ?? [])
          .map((e) => LogVolumeByHour.fromJson(e))
          .toList(),
    );
  }
}

class LogVolumeByHour {
  final String hour;
  final int count;

  LogVolumeByHour({required this.hour, required this.count});

  factory LogVolumeByHour.fromJson(Map<String, dynamic> json) {
    return LogVolumeByHour(
      hour: json['hour'] ?? '',
      count: json['count'] ?? 0,
    );
  }
}
