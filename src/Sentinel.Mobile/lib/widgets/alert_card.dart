import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import '../models/alert.dart';
import '../theme/app_theme.dart';
import 'severity_badge.dart';

class AlertCard extends StatelessWidget {
  final Alert alert;
  final VoidCallback onTap;

  const AlertCard({super.key, required this.alert, required this.onTap});

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: onTap,
      child: Container(
        margin: const EdgeInsets.only(bottom: 12),
        padding: const EdgeInsets.all(16),
        decoration: BoxDecoration(
          color: AppTheme.bgCard,
          borderRadius: BorderRadius.circular(12),
          border: Border.all(color: AppTheme.severityColor(alert.severity).withValues(alpha: 0.3)),
        ),
        child: Row(
          children: [
            // Risk Score
            Container(
              width: 50,
              height: 50,
              decoration: BoxDecoration(
                color: AppTheme.riskColor(alert.riskScore).withValues(alpha: 0.1),
                borderRadius: BorderRadius.circular(12),
                border: Border.all(color: AppTheme.riskColor(alert.riskScore).withValues(alpha: 0.3)),
              ),
              child: Center(
                child: Text(
                  '${alert.riskScore}',
                  style: TextStyle(
                    color: AppTheme.riskColor(alert.riskScore),
                    fontSize: 22,
                    fontWeight: FontWeight.w800,
                  ),
                ),
              ),
            ),
            const SizedBox(width: 14),
            // Details
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Row(
                    children: [
                      SeverityBadge(severity: alert.severity),
                      const SizedBox(width: 8),
                      Text(alert.category, style: const TextStyle(color: AppTheme.textMuted, fontSize: 11)),
                      const Spacer(),
                      if (alert.isAcknowledged)
                        const Icon(Icons.check_circle, color: AppTheme.emerald, size: 16),
                    ],
                  ),
                  const SizedBox(height: 6),
                  Text(
                    alert.aiExplanation,
                    maxLines: 2,
                    overflow: TextOverflow.ellipsis,
                    style: const TextStyle(color: AppTheme.textPrimary, fontSize: 13),
                  ),
                  const SizedBox(height: 4),
                  Text(
                    _formatTime(alert.createdAt),
                    style: const TextStyle(color: AppTheme.textMuted, fontSize: 10),
                  ),
                ],
              ),
            ),
            const SizedBox(width: 8),
            const Icon(Icons.chevron_right, color: AppTheme.textMuted, size: 20),
          ],
        ),
      ),
    );
  }

  String _formatTime(String dateStr) {
    try {
      final date = DateTime.parse(dateStr).toLocal();
      return DateFormat('dd MMM HH:mm').format(date);
    } catch (_) {
      return dateStr;
    }
  }
}
