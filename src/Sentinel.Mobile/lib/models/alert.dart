class Alert {
  final String id;
  final String logEntryId;
  final int riskScore;
  final String severity;
  final String category;
  final String aiExplanation;
  final bool isAcknowledged;
  final String? acknowledgedAt;
  final String? acknowledgedBy;
  final String createdAt;

  Alert({
    required this.id,
    required this.logEntryId,
    required this.riskScore,
    required this.severity,
    required this.category,
    required this.aiExplanation,
    required this.isAcknowledged,
    this.acknowledgedAt,
    this.acknowledgedBy,
    required this.createdAt,
  });

  factory Alert.fromJson(Map<String, dynamic> json) {
    return Alert(
      id: json['id'] ?? '',
      logEntryId: json['logEntryId'] ?? '',
      riskScore: json['riskScore'] ?? 0,
      severity: json['severity'] ?? 'Low',
      category: json['category'] ?? '',
      aiExplanation: json['aiExplanation'] ?? '',
      isAcknowledged: json['isAcknowledged'] ?? false,
      acknowledgedAt: json['acknowledgedAt'],
      acknowledgedBy: json['acknowledgedBy'],
      createdAt: json['createdAt'] ?? '',
    );
  }
}
