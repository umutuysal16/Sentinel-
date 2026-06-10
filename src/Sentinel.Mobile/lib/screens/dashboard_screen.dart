import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../providers/app_provider.dart';
import '../theme/app_theme.dart';
import '../widgets/summary_card.dart';
import '../widgets/alert_card.dart';
import 'alert_detail_screen.dart';

class DashboardScreen extends StatefulWidget {
  const DashboardScreen({super.key});

  @override
  State<DashboardScreen> createState() => _DashboardScreenState();
}

class _DashboardScreenState extends State<DashboardScreen> {
  @override
  void initState() {
    super.initState();
    final provider = context.read<AppProvider>();
    Future.microtask(() => provider.refreshAll());
  }

  @override
  Widget build(BuildContext context) {
    return Consumer<AppProvider>(
      builder: (context, provider, _) {
        final summary = provider.dashboardSummary;

        return RefreshIndicator(
          onRefresh: provider.refreshAll,
          color: AppTheme.emerald,
          child: ListView(
            padding: const EdgeInsets.all(16),
            children: [
              const Text('Dashboard', style: TextStyle(color: Colors.white, fontSize: 24, fontWeight: FontWeight.bold)),
              const SizedBox(height: 4),
              const Text('Real-time monitoring', style: TextStyle(color: AppTheme.textMuted, fontSize: 13)),
              const SizedBox(height: 20),

              if (summary == null)
                const Center(child: CircularProgressIndicator(color: AppTheme.emerald))
              else ...[
                // Summary Cards
                GridView.count(
                  crossAxisCount: 2,
                  shrinkWrap: true,
                  physics: const NeverScrollableScrollPhysics(),
                  mainAxisSpacing: 12,
                  crossAxisSpacing: 12,
                  childAspectRatio: 1.5,
                  children: [
                    SummaryCard(
                      title: 'Agents',
                      value: '${summary.totalAgents}',
                      subtitle: '${summary.onlineAgents} online',
                      icon: Icons.dns,
                      color: AppTheme.blue,
                    ),
                    SummaryCard(
                      title: 'Logs',
                      value: _formatNumber(summary.totalLogs),
                      subtitle: '${summary.criticalLogs24h} critical (24h)',
                      icon: Icons.article,
                      color: AppTheme.emerald,
                    ),
                    SummaryCard(
                      title: 'Alerts',
                      value: '${summary.unacknowledgedAlerts}',
                      subtitle: '${summary.totalAlerts} total',
                      icon: Icons.warning_amber,
                      color: AppTheme.amber,
                    ),
                    SummaryCard(
                      title: 'Risk Score',
                      value: summary.averageRiskScore > 0 ? '${summary.averageRiskScore}' : '-',
                      subtitle: 'Avg (24h)',
                      icon: Icons.shield,
                      color: AppTheme.red,
                    ),
                  ],
                ),
                const SizedBox(height: 24),

                // Recent Alerts
                const Text('Recent Alerts', style: TextStyle(color: AppTheme.textSecondary, fontSize: 14, fontWeight: FontWeight.w600)),
                const SizedBox(height: 12),
                if (provider.alerts.isEmpty)
                  Container(
                    padding: const EdgeInsets.all(32),
                    decoration: BoxDecoration(
                      color: AppTheme.bgCard,
                      borderRadius: BorderRadius.circular(12),
                      border: Border.all(color: AppTheme.borderColor),
                    ),
                    child: const Center(
                      child: Text('No alerts', style: TextStyle(color: AppTheme.textMuted)),
                    ),
                  )
                else
                  ...provider.alerts.take(5).map((alert) => AlertCard(
                    alert: alert,
                    onTap: () => Navigator.push(context, MaterialPageRoute(
                      builder: (_) => AlertDetailScreen(alertId: alert.id),
                    )),
                  )),
              ],
            ],
          ),
        );
      },
    );
  }

  String _formatNumber(int n) {
    if (n >= 1000) return '${(n / 1000).toStringAsFixed(1)}k';
    return '$n';
  }
}
