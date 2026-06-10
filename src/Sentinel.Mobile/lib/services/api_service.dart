import 'dart:convert';
import 'package:http/http.dart' as http;
import '../models/agent.dart';
import '../models/alert_detail.dart';
import '../models/dashboard_summary.dart';

class ApiService {
  final String baseUrl;
  final http.Client _client = http.Client();

  ApiService({required this.baseUrl});

  Future<DashboardSummary> getDashboardSummary() async {
    final response = await _client.get(Uri.parse('$baseUrl/api/dashboard/summary'));
    _checkResponse(response);
    return DashboardSummary.fromJson(json.decode(response.body));
  }

  Future<List<Agent>> getAgents() async {
    final response = await _client.get(Uri.parse('$baseUrl/api/agents'));
    _checkResponse(response);
    final list = json.decode(response.body) as List;
    return list.map((e) => Agent.fromJson(e)).toList();
  }

  Future<Map<String, dynamic>> getAlerts({
    String? severity,
    bool? acknowledged,
    int page = 1,
    int pageSize = 20,
  }) async {
    final params = <String, String>{
      'page': page.toString(),
      'pageSize': pageSize.toString(),
    };
    if (severity != null) params['severity'] = severity;
    if (acknowledged != null) params['acknowledged'] = acknowledged.toString();

    final uri = Uri.parse('$baseUrl/api/alerts').replace(queryParameters: params);
    final response = await _client.get(uri);
    _checkResponse(response);
    return json.decode(response.body);
  }

  Future<AlertDetail> getAlertById(String id) async {
    final response = await _client.get(Uri.parse('$baseUrl/api/alerts/$id'));
    _checkResponse(response);
    return AlertDetail.fromJson(json.decode(response.body));
  }

  Future<void> acknowledgeAlert(String id) async {
    final response = await _client.post(Uri.parse('$baseUrl/api/alerts/$id/acknowledge'));
    _checkResponse(response);
  }

  Future<Map<String, dynamic>> getLogs({
    String? agentId,
    int? level,
    int page = 1,
    int pageSize = 30,
  }) async {
    final params = <String, String>{
      'page': page.toString(),
      'pageSize': pageSize.toString(),
    };
    if (agentId != null) params['agentId'] = agentId;
    if (level != null) params['level'] = level.toString();

    final uri = Uri.parse('$baseUrl/api/logs').replace(queryParameters: params);
    final response = await _client.get(uri);
    _checkResponse(response);
    return json.decode(response.body);
  }

  void _checkResponse(http.Response response) {
    if (response.statusCode < 200 || response.statusCode >= 300) {
      throw Exception('API Error: ${response.statusCode} - ${response.body}');
    }
  }
}
