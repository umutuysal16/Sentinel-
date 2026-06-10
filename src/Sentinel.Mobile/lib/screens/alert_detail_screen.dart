import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import 'package:provider/provider.dart';
import '../models/alert_detail.dart';
import '../providers/app_provider.dart';
import '../theme/app_theme.dart';
import '../widgets/metric_bar.dart';
import '../widgets/severity_badge.dart';

class AlertDetailScreen extends StatefulWidget {
  final String alertId;
  const AlertDetailScreen({super.key, required this.alertId});

  @override
  State<AlertDetailScreen> createState() => _AlertDetailScreenState();
}

class _AlertDetailScreenState extends State<AlertDetailScreen> {
  AlertDetail? _detail;
  bool _loading = true;

  @override
  void initState() {
    super.initState();
    _loadDetail();
  }

  Future<void> _loadDetail() async {
    try {
      final api = context.read<AppProvider>().apiService;
      final detail = await api.getAlertById(widget.alertId);
      setState(() { _detail = detail; _loading = false; });
    } catch (e) {
      setState(() { _loading = false; });
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppTheme.bgPrimary,
      appBar: AppBar(
        title: const Text('Alert Detail'),
        backgroundColor: AppTheme.bgSecondary,
      ),
      body: _loading
        ? const Center(child: CircularProgressIndicator(color: AppTheme.emerald))
        : _detail == null
          ? const Center(child: Text('Failed to load', style: TextStyle(color: AppTheme.textMuted)))
          : _buildContent(),
    );
  }

  Widget _buildContent() {
    final d = _detail!;
    final props = d.relatedLog.properties;

    return ListView(
      padding: const EdgeInsets.all(16),
      children: [
        // Risk Score Hero
        Container(
          padding: const EdgeInsets.all(20),
          decoration: BoxDecoration(
            gradient: LinearGradient(
              colors: [AppTheme.severityColor(d.severity).withValues(alpha: 0.2), AppTheme.bgCard],
            ),
            borderRadius: BorderRadius.circular(16),
            border: Border.all(color: AppTheme.severityColor(d.severity).withValues(alpha: 0.3)),
          ),
          child: Row(
            children: [
              Container(
                width: 80, height: 80,
                decoration: BoxDecoration(
                  shape: BoxShape.circle,
                  color: AppTheme.riskColor(d.riskScore).withValues(alpha: 0.1),
                  border: Border.all(color: AppTheme.riskColor(d.riskScore).withValues(alpha: 0.4), width: 3),
                ),
                child: Center(
                  child: Column(
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      Text('${d.riskScore}', style: TextStyle(color: AppTheme.riskColor(d.riskScore), fontSize: 28, fontWeight: FontWeight.w900)),
                      Text('/10', style: TextStyle(color: AppTheme.textMuted, fontSize: 11)),
                    ],
                  ),
                ),
              ),
              const SizedBox(width: 16),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Row(children: [
                      SeverityBadge(severity: d.severity),
                      const SizedBox(width: 8),
                      Container(
                        padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 3),
                        decoration: BoxDecoration(color: AppTheme.bgInput, borderRadius: BorderRadius.circular(6)),
                        child: Text(d.category, style: const TextStyle(color: AppTheme.textSecondary, fontSize: 11)),
                      ),
                    ]),
                    const SizedBox(height: 8),
                    Text(_formatTime(d.createdAt), style: const TextStyle(color: AppTheme.textMuted, fontSize: 11)),
                    Text('Agent: ${d.agent.name}', style: const TextStyle(color: AppTheme.textSecondary, fontSize: 12)),
                  ],
                ),
              ),
            ],
          ),
        ),
        const SizedBox(height: 16),

        // Acknowledge Button
        if (!d.isAcknowledged)
          Padding(
            padding: const EdgeInsets.only(bottom: 16),
            child: ElevatedButton.icon(
              onPressed: () async {
                await context.read<AppProvider>().acknowledgeAlert(d.id);
                _loadDetail();
              },
              icon: const Icon(Icons.check_circle),
              label: const Text('Acknowledge Alert'),
              style: ElevatedButton.styleFrom(
                backgroundColor: AppTheme.emerald,
                foregroundColor: Colors.white,
                padding: const EdgeInsets.symmetric(vertical: 14),
                shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
              ),
            ),
          )
        else
          Container(
            padding: const EdgeInsets.all(12),
            margin: const EdgeInsets.only(bottom: 16),
            decoration: BoxDecoration(
              color: AppTheme.emerald.withValues(alpha: 0.1),
              borderRadius: BorderRadius.circular(12),
            ),
            child: Row(
              children: [
                const Icon(Icons.check_circle, color: AppTheme.emerald, size: 18),
                const SizedBox(width: 8),
                Text('Acknowledged by ${d.acknowledgedBy ?? "admin"}', style: const TextStyle(color: AppTheme.emerald, fontSize: 13)),
              ],
            ),
          ),

        // AI Explanation
        _section(
          icon: Icons.psychology,
          iconColor: AppTheme.purple,
          title: 'AI ANALYSIS',
          child: Text(d.aiExplanation, style: const TextStyle(color: AppTheme.textPrimary, fontSize: 14, height: 1.5)),
        ),
        const SizedBox(height: 16),

        // Related Log
        _section(
          icon: Icons.warning_amber,
          iconColor: AppTheme.red,
          title: 'TRIGGERING LOG',
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Row(children: [
                Container(
                  padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 3),
                  decoration: BoxDecoration(
                    color: AppTheme.levelColor(d.relatedLog.level).withValues(alpha: 0.2),
                    borderRadius: BorderRadius.circular(6),
                  ),
                  child: Text(d.relatedLog.level, style: TextStyle(color: AppTheme.levelColor(d.relatedLog.level), fontSize: 11, fontWeight: FontWeight.w600)),
                ),
                const SizedBox(width: 8),
                Text(d.relatedLog.source, style: const TextStyle(color: AppTheme.textMuted, fontSize: 11)),
              ]),
              const SizedBox(height: 8),
              Container(
                width: double.infinity,
                padding: const EdgeInsets.all(12),
                decoration: BoxDecoration(color: AppTheme.bgInput, borderRadius: BorderRadius.circular(8)),
                child: Text(d.relatedLog.message, style: const TextStyle(color: Colors.white, fontSize: 13)),
              ),
            ],
          ),
        ),
        const SizedBox(height: 16),

        // System Metrics
        if (props.isNotEmpty)
          _section(
            icon: Icons.bar_chart,
            iconColor: AppTheme.emerald,
            title: 'SYSTEM METRICS',
            child: Column(
              children: [
                if (props['cpu_percent'] != null)
                  Padding(padding: const EdgeInsets.only(bottom: 12), child: MetricBar(
                    label: 'CPU', value: (props['cpu_percent'] as num).toDouble(), max: 100,
                    color: (props['cpu_percent'] as num) > 80 ? AppTheme.red : (props['cpu_percent'] as num) > 60 ? AppTheme.amber : AppTheme.emerald,
                    suffix: '%',
                  )),
                if (props['memory_percent'] != null)
                  Padding(padding: const EdgeInsets.only(bottom: 12), child: MetricBar(
                    label: 'Memory', value: (props['memory_percent'] as num).toDouble(), max: 100,
                    color: (props['memory_percent'] as num) > 80 ? AppTheme.red : (props['memory_percent'] as num) > 60 ? AppTheme.amber : AppTheme.emerald,
                    suffix: '%',
                  )),
                if (props['disk_io_mbps'] != null)
                  Padding(padding: const EdgeInsets.only(bottom: 12), child: MetricBar(
                    label: 'Disk I/O', value: (props['disk_io_mbps'] as num).toDouble(), max: 100, color: AppTheme.blue,
                  )),
                if (props['active_connections'] != null)
                  MetricBar(
                    label: 'Connections', value: (props['active_connections'] as num).toDouble(), max: 500, color: AppTheme.purple,
                  ),
              ],
            ),
          ),
        const SizedBox(height: 16),

        // Agent Timeline
        _section(
          icon: Icons.timeline,
          iconColor: AppTheme.blue,
          title: 'AGENT TIMELINE',
          child: Column(
            children: d.agentTimeline.map((log) {
              final isTriggering = log.id == d.relatedLog.id;
              return Container(
                margin: const EdgeInsets.only(bottom: 6),
                padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 8),
                decoration: BoxDecoration(
                  color: isTriggering ? AppTheme.red.withValues(alpha: 0.1) : Colors.transparent,
                  borderRadius: BorderRadius.circular(8),
                  border: isTriggering ? Border.all(color: AppTheme.red.withValues(alpha: 0.3)) : null,
                ),
                child: Row(
                  children: [
                    Text(_formatTimeShort(log.timestamp), style: const TextStyle(color: AppTheme.textMuted, fontSize: 10, fontFamily: 'monospace')),
                    const SizedBox(width: 8),
                    Container(
                      width: 60,
                      alignment: Alignment.center,
                      padding: const EdgeInsets.symmetric(horizontal: 4, vertical: 2),
                      decoration: BoxDecoration(
                        color: AppTheme.levelColor(log.level).withValues(alpha: 0.2),
                        borderRadius: BorderRadius.circular(4),
                      ),
                      child: Text(log.level, style: TextStyle(color: AppTheme.levelColor(log.level), fontSize: 9, fontWeight: FontWeight.w600)),
                    ),
                    const SizedBox(width: 8),
                    Expanded(child: Text(log.message, maxLines: 1, overflow: TextOverflow.ellipsis, style: const TextStyle(color: AppTheme.textSecondary, fontSize: 11))),
                  ],
                ),
              );
            }).toList(),
          ),
        ),
      ],
    );
  }

  Widget _section({required IconData icon, required Color iconColor, required String title, required Widget child}) {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: AppTheme.bgCard,
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: AppTheme.borderColor),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(children: [
            Icon(icon, color: iconColor, size: 18),
            const SizedBox(width: 8),
            Text(title, style: TextStyle(color: iconColor, fontSize: 12, fontWeight: FontWeight.w700, letterSpacing: 0.5)),
          ]),
          const SizedBox(height: 12),
          child,
        ],
      ),
    );
  }

  String _formatTime(String dateStr) {
    try { return DateFormat('dd MMM yyyy HH:mm').format(DateTime.parse(dateStr).toLocal()); }
    catch (_) { return dateStr; }
  }

  String _formatTimeShort(String dateStr) {
    try { return DateFormat('HH:mm:ss').format(DateTime.parse(dateStr).toLocal()); }
    catch (_) { return dateStr; }
  }
}
