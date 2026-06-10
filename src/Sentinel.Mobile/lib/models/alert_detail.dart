import 'agent.dart';
import 'log_entry.dart';

class AlertDetail {
  final String id;
  final int riskScore;
  final String severity;
  final String category;
  final String aiExplanation;
  final bool isAcknowledged;
  final String? acknowledgedAt;
  final String? acknowledgedBy;
  final String createdAt;
  final LogEntry relatedLog;
  final Agent agent;
  final List<LogEntry> agentTimeline;

  AlertDetail({
    required this.id,
    required this.riskScore,
    required this.severity,
    required this.category,
    required this.aiExplanation,
    required this.isAcknowledged,
    this.acknowledgedAt,
    this.acknowledgedBy,
    required this.createdAt,
    required this.relatedLog,
    required this.agent,
    required this.agentTimeline,
  });

  factory AlertDetail.fromJson(Map<String, dynamic> json) {
    return AlertDetail(
      id: json['id'] ?? '',
      riskScore: json['riskScore'] ?? 0,
      severity: json['severity'] ?? 'Low',
      category: json['category'] ?? '',
      aiExplanation: json['aiExplanation'] ?? '',
      isAcknowledged: json['isAcknowledged'] ?? false,
      acknowledgedAt: json['acknowledgedAt'],
      acknowledgedBy: json['acknowledgedBy'],
      createdAt: json['createdAt'] ?? '',
      relatedLog: LogEntry.fromJson(json['relatedLog'] ?? {}),
      agent: Agent.fromJson(json['agent'] ?? {}),
      agentTimeline: (json['agentTimeline'] as List? ?? [])
          .map((e) => LogEntry.fromJson(e))
          .toList(),
    );
  }
}
