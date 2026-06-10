import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../models/log_entry.dart';
import '../providers/app_provider.dart';
import '../theme/app_theme.dart';
import 'package:intl/intl.dart';

class AgentDetailScreen extends StatefulWidget {
  final String agentId;
  final String agentName;

  const AgentDetailScreen({super.key, required this.agentId, required this.agentName});

  @override
  State<AgentDetailScreen> createState() => _AgentDetailScreenState();
}

class _AgentDetailScreenState extends State<AgentDetailScreen> {
  List<LogEntry> _logs = [];
  bool _loading = true;

  @override
  void initState() {
    super.initState();
    _loadLogs();
  }

  Future<void> _loadLogs() async {
    try {
      final api = context.read<AppProvider>().apiService;
      final data = await api.getLogs(agentId: widget.agentId, pageSize: 50);
      final items = data['items'] as List;
      setState(() {
        _logs = items.map((e) => LogEntry.fromJson(e)).toList();
        _loading = false;
      });
    } catch (e) {
      setState(() { _loading = false; });
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppTheme.bgPrimary,
      appBar: AppBar(
        title: Text(widget.agentName),
        backgroundColor: AppTheme.bgSecondary,
      ),
      body: _loading
        ? const Center(child: CircularProgressIndicator(color: AppTheme.emerald))
        : RefreshIndicator(
            onRefresh: _loadLogs,
            color: AppTheme.emerald,
            child: ListView(
              padding: const EdgeInsets.all(16),
              children: [
                Text('Logs from ${widget.agentName}',
                    style: const TextStyle(color: AppTheme.textSecondary, fontSize: 14, fontWeight: FontWeight.w600)),
                const SizedBox(height: 4),
                Text('${_logs.length} entries', style: const TextStyle(color: AppTheme.textMuted, fontSize: 12)),
                const SizedBox(height: 16),

                ..._logs.map((log) => Container(
                  margin: const EdgeInsets.only(bottom: 8),
                  padding: const EdgeInsets.all(12),
                  decoration: BoxDecoration(
                    color: AppTheme.bgCard,
                    borderRadius: BorderRadius.circular(10),
                    border: Border.all(color: AppTheme.borderColor),
                  ),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Row(
                        children: [
                          Container(
                            padding: const EdgeInsets.symmetric(horizontal: 6, vertical: 2),
                            decoration: BoxDecoration(
                              color: AppTheme.levelColor(log.level).withValues(alpha: 0.2),
                              borderRadius: BorderRadius.circular(4),
                            ),
                            child: Text(log.level, style: TextStyle(color: AppTheme.levelColor(log.level), fontSize: 10, fontWeight: FontWeight.w600)),
                          ),
                          const SizedBox(width: 8),
                          Text(log.source, style: const TextStyle(color: AppTheme.textMuted, fontSize: 11)),
                          const Spacer(),
                          Text(_formatTime(log.timestamp), style: const TextStyle(color: AppTheme.textMuted, fontSize: 10, fontFamily: 'monospace')),
                        ],
                      ),
                      const SizedBox(height: 6),
                      Text(log.message, style: const TextStyle(color: AppTheme.textPrimary, fontSize: 13)),
                    ],
                  ),
                )),
              ],
            ),
          ),
    );
  }

  String _formatTime(String dateStr) {
    try { return DateFormat('HH:mm:ss').format(DateTime.parse(dateStr).toLocal()); }
    catch (_) { return dateStr; }
  }
}
