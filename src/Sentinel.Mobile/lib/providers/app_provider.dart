import 'package:flutter/material.dart';
import '../models/agent.dart';
import '../models/alert.dart';
import '../models/dashboard_summary.dart';
import '../services/api_service.dart';
import '../services/signalr_service.dart';

class AppProvider extends ChangeNotifier {
  final ApiService apiService;
  final SignalRService signalRService;

  DashboardSummary? dashboardSummary;
  List<Agent> agents = [];
  List<Alert> alerts = [];
  bool isLoading = false;
  String? error;

  AppProvider({required this.apiService, required this.signalRService}) {
    _initSignalR();
  }

  void _initSignalR() {
    signalRService.onAlertReceived = (_) {
      refreshAll();
    };
    signalRService.onAgentStatusReceived = (_) {
      loadAgents();
    };
    signalRService.connect();
  }

  Future<void> refreshAll() async {
    await Future.wait([
      loadDashboard(),
      loadAlerts(),
      loadAgents(),
    ]);
  }

  Future<void> loadDashboard() async {
    try {
      dashboardSummary = await apiService.getDashboardSummary();
      notifyListeners();
    } catch (e) {
      error = e.toString();
      notifyListeners();
    }
  }

  Future<void> loadAgents() async {
    try {
      agents = await apiService.getAgents();
      notifyListeners();
    } catch (e) {
      error = e.toString();
      notifyListeners();
    }
  }

  Future<void> loadAlerts() async {
    try {
      isLoading = true;
      notifyListeners();

      final data = await apiService.getAlerts(pageSize: 50);
      final items = data['items'] as List;
      alerts = items.map((e) => Alert.fromJson(e)).toList();

      isLoading = false;
      notifyListeners();
    } catch (e) {
      isLoading = false;
      error = e.toString();
      notifyListeners();
    }
  }

  Future<void> acknowledgeAlert(String id) async {
    try {
      await apiService.acknowledgeAlert(id);
      await loadAlerts();
      await loadDashboard();
    } catch (e) {
      error = e.toString();
      notifyListeners();
    }
  }
}
