class LogEntry {
  final String id;
  final String agentId;
  final String agentName;
  final String source;
  final String level;
  final String message;
  final String? stackTrace;
  final Map<String, dynamic> properties;
  final String timestamp;

  LogEntry({
    required this.id,
    required this.agentId,
    required this.agentName,
    required this.source,
    required this.level,
    required this.message,
    this.stackTrace,
    required this.properties,
    required this.timestamp,
  });

  factory LogEntry.fromJson(Map<String, dynamic> json) {
    return LogEntry(
      id: json['id'] ?? '',
      agentId: json['agentId'] ?? '',
      agentName: json['agentName'] ?? '',
      source: json['source'] ?? '',
      level: json['level'] ?? 'Information',
      message: json['message'] ?? '',
      stackTrace: json['stackTrace'],
      properties: Map<String, dynamic>.from(json['properties'] ?? {}),
      timestamp: json['timestamp'] ?? '',
    );
  }
}
