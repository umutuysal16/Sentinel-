import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../providers/app_provider.dart';
import '../theme/app_theme.dart';
import '../widgets/alert_card.dart';
import 'alert_detail_screen.dart';

class AlertsScreen extends StatefulWidget {
  const AlertsScreen({super.key});

  @override
  State<AlertsScreen> createState() => _AlertsScreenState();
}

class _AlertsScreenState extends State<AlertsScreen> {
  @override
  void initState() {
    super.initState();
    final provider = context.read<AppProvider>();
    Future.microtask(() => provider.loadAlerts());
  }

  @override
  Widget build(BuildContext context) {
    return Consumer<AppProvider>(
      builder: (context, provider, _) {
        return RefreshIndicator(
          onRefresh: provider.loadAlerts,
          color: AppTheme.emerald,
          child: ListView(
            padding: const EdgeInsets.all(16),
            children: [
              const Text('Security Alerts', style: TextStyle(color: Colors.white, fontSize: 24, fontWeight: FontWeight.bold)),
              const SizedBox(height: 4),
              const Text('AI-powered threat detection', style: TextStyle(color: AppTheme.textMuted, fontSize: 13)),
              const SizedBox(height: 20),

              if (provider.isLoading)
                const Center(child: CircularProgressIndicator(color: AppTheme.emerald))
              else if (provider.alerts.isEmpty)
                Container(
                  padding: const EdgeInsets.all(48),
                  decoration: BoxDecoration(
                    color: AppTheme.bgCard,
                    borderRadius: BorderRadius.circular(12),
                    border: Border.all(color: AppTheme.borderColor),
                  ),
                  child: const Column(
                    children: [
                      Icon(Icons.check_circle_outline, color: AppTheme.emerald, size: 48),
                      SizedBox(height: 12),
                      Text('No alerts', style: TextStyle(color: AppTheme.textMuted, fontSize: 15)),
                      Text('System is running smoothly', style: TextStyle(color: AppTheme.textMuted, fontSize: 12)),
                    ],
                  ),
                )
              else
                ...provider.alerts.map((alert) => AlertCard(
                  alert: alert,
                  onTap: () => Navigator.push(context, MaterialPageRoute(
                    builder: (_) => AlertDetailScreen(alertId: alert.id),
                  )),
                )),
            ],
          ),
        );
      },
    );
  }
}
