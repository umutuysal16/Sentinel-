import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../providers/app_provider.dart';
import '../theme/app_theme.dart';
import '../widgets/agent_card.dart';
import 'agent_detail_screen.dart';

class AgentsScreen extends StatefulWidget {
  const AgentsScreen({super.key});

  @override
  State<AgentsScreen> createState() => _AgentsScreenState();
}

class _AgentsScreenState extends State<AgentsScreen> {
  @override
  void initState() {
    super.initState();
    final provider = context.read<AppProvider>();
    Future.microtask(() => provider.loadAgents());
  }

  @override
  Widget build(BuildContext context) {
    return Consumer<AppProvider>(
      builder: (context, provider, _) {
        return RefreshIndicator(
          onRefresh: provider.loadAgents,
          color: AppTheme.emerald,
          child: ListView(
            padding: const EdgeInsets.all(16),
            children: [
              const Text('Agents', style: TextStyle(color: Colors.white, fontSize: 24, fontWeight: FontWeight.bold)),
              const SizedBox(height: 4),
              const Text('Monitored devices & servers', style: TextStyle(color: AppTheme.textMuted, fontSize: 13)),
              const SizedBox(height: 20),

              if (provider.agents.isEmpty)
                Container(
                  padding: const EdgeInsets.all(48),
                  decoration: BoxDecoration(
                    color: AppTheme.bgCard,
                    borderRadius: BorderRadius.circular(12),
                    border: Border.all(color: AppTheme.borderColor),
                  ),
                  child: const Column(
                    children: [
                      Icon(Icons.dns, color: AppTheme.textMuted, size: 48),
                      SizedBox(height: 12),
                      Text('No agents', style: TextStyle(color: AppTheme.textMuted)),
                    ],
                  ),
                )
              else
                ...provider.agents.map((agent) => Padding(
                  padding: const EdgeInsets.only(bottom: 12),
                  child: AgentCard(
                    agent: agent,
                    onTap: () => Navigator.push(context, MaterialPageRoute(
                      builder: (_) => AgentDetailScreen(agentId: agent.id, agentName: agent.name),
                    )),
                  ),
                )),
            ],
          ),
        );
      },
    );
  }
}
