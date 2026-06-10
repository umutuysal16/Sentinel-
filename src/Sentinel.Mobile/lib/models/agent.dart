class Agent {
  final String id;
  final String name;
  final String deviceType;
  final String status;
  final String lastHeartbeat;
  final String? ipAddress;
  final int logCount;

  Agent({
    required this.id,
    required this.name,
    required this.deviceType,
    required this.status,
    required this.lastHeartbeat,
    this.ipAddress,
    required this.logCount,
  });

  factory Agent.fromJson(Map<String, dynamic> json) {
    return Agent(
      id: json['id'] ?? '',
      name: json['name'] ?? '',
      deviceType: json['deviceType'] ?? '',
      status: json['status'] ?? 'Offline',
      lastHeartbeat: json['lastHeartbeat'] ?? '',
      ipAddress: json['ipAddress'],
      logCount: json['logCount'] ?? 0,
    );
  }
}
