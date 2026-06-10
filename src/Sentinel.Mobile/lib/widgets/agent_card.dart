import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import '../models/agent.dart';
import '../theme/app_theme.dart';

class AgentCard extends StatelessWidget {
  final Agent agent;
  final VoidCallback onTap;

  const AgentCard({super.key, required this.agent, required this.onTap});

  @override
  Widget build(BuildContext context) {
    final statusColor = AppTheme.statusColor(agent.status);

    return GestureDetector(
      onTap: onTap,
      child: Container(
        padding: const EdgeInsets.all(16),
        decoration: BoxDecoration(
          color: AppTheme.bgCard,
          borderRadius: BorderRadius.circular(12),
          border: Border.all(color: statusColor.withValues(alpha: 0.2)),
        ),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                Icon(Icons.dns, color: statusColor, size: 24),
                const SizedBox(width: 10),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(agent.name, style: const TextStyle(color: Colors.white, fontWeight: FontWeight.w600, fontSize: 15)),
                      Text(agent.deviceType, style: const TextStyle(color: AppTheme.textMuted, fontSize: 11)),
                    ],
                  ),
                ),
                Container(
                  padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
                  decoration: BoxDecoration(
                    color: statusColor.withValues(alpha: 0.1),
                    borderRadius: BorderRadius.circular(8),
                  ),
                  child: Row(
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      Container(
                        width: 6, height: 6,
                        decoration: BoxDecoration(color: statusColor, shape: BoxShape.circle),
                      ),
                      const SizedBox(width: 4),
                      Text(agent.status, style: TextStyle(color: statusColor, fontSize: 11, fontWeight: FontWeight.w500)),
                    ],
                  ),
                ),
              ],
            ),
            const SizedBox(height: 14),
            Row(
              children: [
                _stat('Logs', agent.logCount.toString()),
                const SizedBox(width: 24),
                _stat('Heartbeat', _formatTime(agent.lastHeartbeat)),
              ],
            ),
          ],
        ),
      ),
    );
  }

  Widget _stat(String label, String value) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(label, style: const TextStyle(color: AppTheme.textMuted, fontSize: 10)),
        const SizedBox(height: 2),
        Text(value, style: const TextStyle(color: Colors.white, fontSize: 13, fontWeight: FontWeight.w500)),
      ],
    );
  }

  String _formatTime(String dateStr) {
    try {
      final date = DateTime.parse(dateStr).toLocal();
      return DateFormat('HH:mm:ss').format(date);
    } catch (_) {
      return dateStr;
    }
  }
}
