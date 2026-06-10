import 'package:flutter/material.dart';
import 'package:google_fonts/google_fonts.dart';

class AppTheme {
  static const bgPrimary = Color(0xFF0F172A);
  static const bgSecondary = Color(0xFF1E293B);
  static const bgCard = Color(0xFF1E293B);
  static const bgInput = Color(0xFF334155);
  static const borderColor = Color(0xFF334155);
  static const textPrimary = Color(0xFFE2E8F0);
  static const textSecondary = Color(0xFF94A3B8);
  static const textMuted = Color(0xFF64748B);
  static const emerald = Color(0xFF10B981);
  static const amber = Color(0xFFF59E0B);
  static const red = Color(0xFFEF4444);
  static const blue = Color(0xFF3B82F6);
  static const purple = Color(0xFFA855F7);
  static const orange = Color(0xFFF97316);

  static Color severityColor(String severity) {
    switch (severity) {
      case 'Critical': return red;
      case 'High': return orange;
      case 'Medium': return amber;
      case 'Low': return blue;
      default: return textMuted;
    }
  }

  static Color statusColor(String status) {
    switch (status) {
      case 'Online': return emerald;
      case 'Degraded': return amber;
      case 'Offline': return red;
      default: return textMuted;
    }
  }

  static Color levelColor(String level) {
    switch (level) {
      case 'Critical': return red;
      case 'Error': return red;
      case 'Warning': return amber;
      case 'Information': return blue;
      case 'Debug': return textMuted;
      default: return textMuted;
    }
  }

  static Color riskColor(int score) {
    if (score >= 9) return red;
    if (score >= 6) return orange;
    if (score >= 4) return amber;
    return blue;
  }

  static ThemeData get darkTheme {
    return ThemeData(
      brightness: Brightness.dark,
      scaffoldBackgroundColor: bgPrimary,
      textTheme: GoogleFonts.interTextTheme(ThemeData.dark().textTheme),
      colorScheme: const ColorScheme.dark(
        primary: emerald,
        surface: bgSecondary,
      ),
      appBarTheme: const AppBarTheme(
        backgroundColor: bgSecondary,
        elevation: 0,
      ),
      bottomNavigationBarTheme: const BottomNavigationBarThemeData(
        backgroundColor: bgSecondary,
        selectedItemColor: emerald,
        unselectedItemColor: textMuted,
      ),
      cardTheme: CardThemeData(
        color: bgCard,
        elevation: 0,
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(12),
          side: const BorderSide(color: borderColor),
        ),
      ),
    );
  }
}
