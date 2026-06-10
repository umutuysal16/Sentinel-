import 'package:flutter/material.dart';
import '../theme/app_theme.dart';

class SeverityBadge extends StatelessWidget {
  final String severity;
  final double fontSize;

  const SeverityBadge({super.key, required this.severity, this.fontSize = 11});

  @override
  Widget build(BuildContext context) {
    final color = AppTheme.severityColor(severity);
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 3),
      decoration: BoxDecoration(
        color: color.withValues(alpha: 0.2),
        borderRadius: BorderRadius.circular(6),
      ),
      child: Text(
        severity,
        style: TextStyle(color: color, fontSize: fontSize, fontWeight: FontWeight.w600),
      ),
    );
  }
}
