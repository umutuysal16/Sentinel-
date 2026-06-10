import { useQuery } from '@tanstack/react-query';
import { Link } from 'react-router-dom';
import { getAgents } from '../api/client';
import { Server, Wifi, WifiOff, AlertTriangle } from 'lucide-react';
import clsx from 'clsx';

const statusConfig: Record<string, { color: string; icon: typeof Wifi; label: string }> = {
  Online: { color: 'text-emerald-400 bg-emerald-500/10 border-emerald-500/20', icon: Wifi, label: 'Online' },
  Degraded: { color: 'text-amber-400 bg-amber-500/10 border-amber-500/20', icon: AlertTriangle, label: 'Degraded' },
  Offline: { color: 'text-red-400 bg-red-500/10 border-red-500/20', icon: WifiOff, label: 'Offline' },
};

export default function AgentsPage() {
  const { data: agents, isLoading } = useQuery({
    queryKey: ['agents'],
    queryFn: getAgents,
    refetchInterval: 10000,
  });

  return (
    <div className="space-y-6">
      <div>
        <h2 className="text-2xl font-bold text-white mb-1">Agents</h2>
        <p className="text-sm text-slate-400">Monitored IoT devices and servers</p>
      </div>

      {isLoading ? (
        <div className="text-slate-500 text-center py-8">Loading...</div>
      ) : agents?.length === 0 ? (
        <div className="bg-slate-900 border border-slate-700 rounded-xl p-12 text-center">
          <Server className="w-12 h-12 text-slate-600 mx-auto mb-3" />
          <p className="text-slate-500">No agents registered yet.</p>
        </div>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          {agents?.map(agent => {
            const config = statusConfig[agent.status] || statusConfig.Offline;
            const StatusIcon = config.icon;

            return (
              <Link key={agent.id} to={`/agents/${agent.id}`} className={clsx('bg-slate-900 border rounded-xl p-5 block hover:ring-2 hover:ring-emerald-500/30 transition-all', config.color)}>
                <div className="flex items-center justify-between mb-4">
                  <div className="flex items-center gap-3">
                    <Server className="w-6 h-6" />
                    <div>
                      <h3 className="font-semibold text-white">{agent.name}</h3>
                      <p className="text-xs text-slate-400">{agent.deviceType}</p>
                    </div>
                  </div>
                  <div className="flex items-center gap-1.5 text-xs">
                    <StatusIcon className="w-4 h-4" />
                    {config.label}
                  </div>
                </div>

                <div className="grid grid-cols-2 gap-3 text-sm">
                  <div>
                    <p className="text-slate-500 text-xs">Logs Sent</p>
                    <p className="text-white font-medium">{agent.logCount.toLocaleString()}</p>
                  </div>
                  <div>
                    <p className="text-slate-500 text-xs">Last Heartbeat</p>
                    <p className="text-white font-medium text-xs">
                      {new Date(agent.lastHeartbeat).toLocaleTimeString('tr-TR')}
                    </p>
                  </div>
                </div>

                {agent.ipAddress && (
                  <div className="mt-3 pt-3 border-t border-slate-700">
                    <p className="text-xs text-slate-500">IP: {agent.ipAddress}</p>
                  </div>
                )}
              </Link>
            );
          })}
        </div>
      )}
    </div>
  );
}
