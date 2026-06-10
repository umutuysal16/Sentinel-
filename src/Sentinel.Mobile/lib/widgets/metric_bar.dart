import 'package:flutter/material.dart';
import '../theme/app_theme.dart';

class MetricBar extends StatelessWidget {
  final String label;
  final double value;
  final double max;
  final Color color;
  final String? suffix;

  const MetricBar({
    super.key,
    required this.label,
    required this.value,
    required this.max,
    required this.color,
    this.suffix,
  });

  @override
  Widget build(BuildContext context) {
    final percent = (value / max).clamp(0.0, 1.0);
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: [
            Text(label, style: const TextStyle(color: AppTheme.textSecondary, fontSize: 12)),
            Text('${value.toStringAsFixed(1)}${suffix ?? ''}',
                style: const TextStyle(color: Colors.white, fontSize: 12, fontWeight: FontWeight.w500)),
          ],
        ),
        const SizedBox(height: 4),
        ClipRRect(
          borderRadius: BorderRadius.circular(4),
          child: LinearProgressIndicator(
            value: percent,
            backgroundColor: AppTheme.bgInput,
            valueColor: AlwaysStoppedAnimation(color),
            minHeight: 8,
          ),
        ),
      ],
    );
  }
}
