import { useParams, Link } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { getAgents, getLogs } from '../api/client';
import { ArrowLeft, Server, Wifi, WifiOff, AlertTriangle } from 'lucide-react';
import clsx from 'clsx';
import { useState } from 'react';
import type { LogEntry } from '../types';
import LogDetailModal from '../components/logs/LogDetailModal';

const levelColors: Record<string, string> = {
  Debug: 'bg-slate-500/20 text-slate-400',
  Information: 'bg-blue-500/20 text-blue-400',
  Warning: 'bg-amber-500/20 text-amber-400',
  Error: 'bg-red-500/20 text-red-400',
  Critical: 'bg-red-600/30 text-red-300 font-bold',
};

const statusConfig: Record<string, { color: string; icon: typeof Wifi }> = {
  Online: { color: 'text-emerald-400', icon: Wifi },
  Degraded: { color: 'text-amber-400', icon: AlertTriangle },
  Offline: { color: 'text-red-400', icon: WifiOff },
};

export default function AgentDetailPage() {
  const { id } = useParams<{ id: string }>();
  const [selectedLog, setSelectedLog] = useState<LogEntry | null>(null);

  const { data: agents } = useQuery({
    queryKey: ['agents'],
    queryFn: getAgents,
  });

  const agent = agents?.find(a => a.id === id);

  const { data: logsData } = useQuery({
    queryKey: ['logs', id, 'agent-detail'],
    queryFn: () => getLogs({ agentId: id, pageSize: 50 }),
    refetchInterval: 5000,
    enabled: !!id,
  });

  if (!agent) {
    return (
      <div className="space-y-4">
        <Link to="/agents" className="flex items-center gap-2 text-slate-400 hover:text-white transition-colors">
          <ArrowLeft className="w-4 h-4" /> Back to Agents
        </Link>
        <div className="text-slate-500 text-center py-12">Agent not found or loading...</div>
      </div>
    );
  }

  const config = statusConfig[agent.status] || statusConfig.Offline;
  const StatusIcon = config.icon;

  // Calculate level distribution
  const levelCounts: Record<string, number> = {};
  logsData?.items.forEach(log => {
    levelCounts[log.level] = (levelCounts[log.level] || 0) + 1;
  });

  return (
    <div className="space-y-6">
      <Link to="/agents" className="flex items-center gap-2 text-slate-400 hover:text-white transition-colors text-sm">
        <ArrowLeft className="w-4 h-4" /> Back to Agents
      </Link>

      {/* Agent Header */}
      <div className="bg-slate-900 border border-slate-700 rounded-xl p-6">
        <div className="flex items-center justify-between">
          <div className="flex items-center gap-4">
            <Server className="w-10 h-10 text-emerald-400" />
            <div>
              <h2 className="text-2xl font-bold text-white">{agent.name}</h2>
              <p className="text-sm text-slate-400">{agent.deviceType}</p>
            </div>
          </div>
          <div className={clsx('flex items-center gap-2 text-sm', config.color)}>
            <StatusIcon className="w-5 h-5" />
            {agent.status}
          </div>
        </div>

        <div className="grid grid-cols-4 gap-4 mt-6">
          <div className="bg-slate-800 rounded-lg p-3">
            <p className="text-xs text-slate-500">Total Logs</p>
            <p className="text-xl font-bold text-white">{agent.logCount.toLocaleString()}</p>
          </div>
          <div className="bg-slate-800 rounded-lg p-3">
            <p className="text-xs text-slate-500">Last Heartbeat</p>
            <p className="text-sm font-medium text-white">{new Date(agent.lastHeartbeat).toLocaleString('tr-TR')}</p>
          </div>
          <div className="bg-slate-800 rounded-lg p-3">
            <p className="text-xs text-slate-500">IP Address</p>
            <p className="text-sm font-medium text-white">{agent.ipAddress || 'N/A'}</p>
          </div>
          <div className="bg-slate-800 rounded-lg p-3">
            <p className="text-xs text-slate-500">Log Levels</p>
            <div className="flex gap-1 mt-1 flex-wrap">
              {Object.entries(levelCounts).map(([lvl, count]) => (
                <span key={lvl} className={clsx('px-1.5 py-0.5 rounded text-xs', levelColors[lvl])}>
                  {lvl}: {count}
                </span>
              ))}
            </div>
          </div>
        </div>
      </div>

      {/* Agent Logs */}
      <div className="bg-slate-900 border border-slate-700 rounded-xl overflow-hidden">
        <div className="px-5 py-3 border-b border-slate-700">
          <h3 className="text-sm font-medium text-slate-400">Recent Logs from {agent.name}</h3>
        </div>
        <table className="w-full text-sm">
          <thead>
            <tr className="border-b border-slate-700 text-slate-400">
              <th className="text-left px-4 py-3 w-24">Level</th>
              <th className="text-left px-4 py-3 w-36">Time</th>
              <th className="text-left px-4 py-3 w-36">Source</th>
              <th className="text-left px-4 py-3">Message</th>
            </tr>
          </thead>
          <tbody>
            {logsData?.items.map(log => (
              <tr key={log.id} className="border-b border-slate-800 hover:bg-slate-800/50 transition-colors cursor-pointer" onClick={() => setSelectedLog(log)}>
                <td className="px-4 py-2">
                  <span className={clsx('px-2 py-0.5 rounded text-xs font-medium', levelColors[log.level])}>
                    {log.level}
                  </span>
                </td>
                <td className="px-4 py-2 text-slate-400 text-xs font-mono">
                  {new Date(log.timestamp).toLocaleTimeString('tr-TR', { hour: '2-digit', minute: '2-digit', second: '2-digit' })}
                </td>
                <td className="px-4 py-2 text-slate-400">{log.source}</td>
                <td className="px-4 py-2 text-slate-200 truncate max-w-lg">{log.message}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {selectedLog && (
        <LogDetailModal log={selectedLog} onClose={() => setSelectedLog(null)} />
      )}
    </div>
  );
}
