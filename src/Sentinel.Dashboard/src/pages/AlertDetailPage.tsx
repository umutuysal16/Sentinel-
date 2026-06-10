import { useParams, Link } from 'react-router-dom';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { getAlertById, acknowledgeAlert } from '../api/client';
import { ArrowLeft, ShieldAlert, Server, Clock, CheckCircle, Brain, Activity } from 'lucide-react';
import clsx from 'clsx';

const severityColor: Record<string, string> = {
  Low: 'from-blue-500/20 to-blue-600/10 border-blue-500/30',
  Medium: 'from-amber-500/20 to-amber-600/10 border-amber-500/30',
  High: 'from-orange-500/20 to-orange-600/10 border-orange-500/30',
  Critical: 'from-red-500/20 to-red-600/10 border-red-500/30',
};

const severityBadge: Record<string, string> = {
  Low: 'bg-blue-500/20 text-blue-400',
  Medium: 'bg-amber-500/20 text-amber-400',
  High: 'bg-orange-500/20 text-orange-400',
  Critical: 'bg-red-500/20 text-red-400',
};

const levelColors: Record<string, string> = {
  Debug: 'bg-slate-500/20 text-slate-400',
  Information: 'bg-blue-500/20 text-blue-400',
  Warning: 'bg-amber-500/20 text-amber-400',
  Error: 'bg-red-500/20 text-red-400',
  Critical: 'bg-red-600/30 text-red-300 font-bold',
};

const riskColor = (score: number) => {
  if (score <= 3) return 'text-blue-400';
  if (score <= 5) return 'text-amber-400';
  if (score <= 8) return 'text-orange-400';
  return 'text-red-400';
};

const riskBg = (score: number) => {
  if (score <= 3) return 'bg-blue-500/10 border-blue-500/20';
  if (score <= 5) return 'bg-amber-500/10 border-amber-500/20';
  if (score <= 8) return 'bg-orange-500/10 border-orange-500/20';
  return 'bg-red-500/10 border-red-500/20';
};

function MetricBar({ label, value, max, color }: { label: string; value: number; max: number; color: string }) {
  const percent = Math.min(100, (value / max) * 100);
  return (
    <div>
      <div className="flex justify-between text-xs mb-1">
        <span className="text-slate-400">{label}</span>
        <span className="text-white font-medium">{value}{max === 100 ? '%' : ''}</span>
      </div>
      <div className="h-2.5 bg-slate-700 rounded-full overflow-hidden">
        <div className={clsx('h-full rounded-full transition-all duration-500', color)} style={{ width: `${percent}%` }} />
      </div>
    </div>
  );
}

export default function AlertDetailPage() {
  const { id } = useParams<{ id: string }>();
  const queryClient = useQueryClient();

  const { data: alert, isLoading } = useQuery({
    queryKey: ['alert-detail', id],
    queryFn: () => getAlertById(id!),
    enabled: !!id,
  });

  const ackMutation = useMutation({
    mutationFn: acknowledgeAlert,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['alert-detail', id] });
      queryClient.invalidateQueries({ queryKey: ['alerts'] });
    },
  });

  if (isLoading || !alert) {
    return (
      <div className="space-y-4">
        <Link to="/alerts" className="flex items-center gap-2 text-slate-400 hover:text-white transition-colors text-sm">
          <ArrowLeft className="w-4 h-4" /> Back to Alerts
        </Link>
        <div className="text-slate-500 text-center py-12">Loading alert details...</div>
      </div>
    );
  }

  const props = alert.relatedLog.properties || {};

  return (
    <div className="space-y-6">
      <Link to="/alerts" className="flex items-center gap-2 text-slate-400 hover:text-white transition-colors text-sm">
        <ArrowLeft className="w-4 h-4" /> Back to Alerts
      </Link>

      {/* Risk Score Hero */}
      <div className={clsx('bg-gradient-to-r border rounded-xl p-6', severityColor[alert.severity])}>
        <div className="flex items-center gap-8">
          {/* Risk Score Circle */}
          <div className={clsx('w-28 h-28 rounded-full border-4 flex flex-col items-center justify-center', riskBg(alert.riskScore))}>
            <span className={clsx('text-4xl font-black', riskColor(alert.riskScore))}>{alert.riskScore}</span>
            <span className="text-xs text-slate-400">/10</span>
          </div>

          {/* Alert Info */}
          <div className="flex-1">
            <div className="flex items-center gap-3 mb-2">
              <span className={clsx('px-3 py-1 rounded-lg text-sm font-semibold', severityBadge[alert.severity])}>
                {alert.severity}
              </span>
              <span className="px-3 py-1 rounded-lg text-sm bg-slate-800 text-slate-300">{alert.category}</span>
            </div>
            <div className="flex items-center gap-4 text-sm text-slate-400 mb-3">
              <span className="flex items-center gap-1"><Clock className="w-4 h-4" /> {new Date(alert.createdAt).toLocaleString('tr-TR')}</span>
              <span className="flex items-center gap-1"><Server className="w-4 h-4" /> {alert.agent.name}</span>
            </div>
            {alert.isAcknowledged ? (
              <span className="flex items-center gap-1 text-emerald-400 text-sm">
                <CheckCircle className="w-4 h-4" /> Acknowledged by {alert.acknowledgedBy} at {new Date(alert.acknowledgedAt!).toLocaleString('tr-TR')}
              </span>
            ) : (
              <button
                onClick={() => ackMutation.mutate(alert.id)}
                disabled={ackMutation.isPending}
                className="px-5 py-2 bg-emerald-600 hover:bg-emerald-500 text-white text-sm font-medium rounded-lg transition-colors disabled:opacity-50"
              >
                Acknowledge Alert
              </button>
            )}
          </div>
        </div>
      </div>

      {/* AI Explanation */}
      <div className="bg-slate-900 border border-slate-700 rounded-xl p-6">
        <div className="flex items-center gap-2 mb-3">
          <Brain className="w-5 h-5 text-purple-400" />
          <h3 className="text-sm font-semibold text-purple-400 uppercase">AI Analysis</h3>
        </div>
        <p className="text-white text-base leading-relaxed">{alert.aiExplanation}</p>
        <div className="mt-3 pt-3 border-t border-slate-700">
          <p className="text-xs text-slate-500">Powered by Google Gemini via n8n orchestration</p>
        </div>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Related Log */}
        <div className="bg-slate-900 border border-slate-700 rounded-xl p-6">
          <div className="flex items-center gap-2 mb-4">
            <ShieldAlert className="w-5 h-5 text-red-400" />
            <h3 className="text-sm font-semibold text-slate-300 uppercase">Triggering Log Entry</h3>
          </div>

          <div className="space-y-3">
            <div className="flex items-center gap-2">
              <span className={clsx('px-2 py-0.5 rounded text-xs font-medium', levelColors[alert.relatedLog.level])}>
                {alert.relatedLog.level}
              </span>
              <span className="text-sm text-slate-400">{alert.relatedLog.source}</span>
            </div>
            <p className="text-white text-sm leading-relaxed bg-slate-800 rounded-lg p-3">{alert.relatedLog.message}</p>
            <p className="text-xs text-slate-500 font-mono">
              {new Date(alert.relatedLog.timestamp).toLocaleString('tr-TR')} | ID: {alert.relatedLog.id.slice(0, 8)}...
            </p>
          </div>
        </div>

        {/* System Metrics */}
        <div className="bg-slate-900 border border-slate-700 rounded-xl p-6">
          <div className="flex items-center gap-2 mb-4">
            <Activity className="w-5 h-5 text-emerald-400" />
            <h3 className="text-sm font-semibold text-slate-300 uppercase">System Metrics at Time of Alert</h3>
          </div>

          {Object.keys(props).length > 0 ? (
            <div className="space-y-4">
              {props.cpu_percent !== undefined && (
                <MetricBar label="CPU Usage" value={Number(props.cpu_percent)} max={100}
                  color={Number(props.cpu_percent) > 80 ? 'bg-red-500' : Number(props.cpu_percent) > 60 ? 'bg-amber-500' : 'bg-emerald-500'} />
              )}
              {props.memory_percent !== undefined && (
                <MetricBar label="Memory Usage" value={Number(props.memory_percent)} max={100}
                  color={Number(props.memory_percent) > 80 ? 'bg-red-500' : Number(props.memory_percent) > 60 ? 'bg-amber-500' : 'bg-emerald-500'} />
              )}
              {props.disk_io_mbps !== undefined && (
                <MetricBar label="Disk I/O (MB/s)" value={Number(props.disk_io_mbps)} max={100} color="bg-blue-500" />
              )}
              {props.network_in_mbps !== undefined && (
                <MetricBar label="Network In (MB/s)" value={Number(props.network_in_mbps)} max={50} color="bg-purple-500" />
              )}
              {props.network_out_mbps !== undefined && (
                <MetricBar label="Network Out (MB/s)" value={Number(props.network_out_mbps)} max={30} color="bg-indigo-500" />
              )}
              {props.active_connections !== undefined && (
                <MetricBar label="Active Connections" value={Number(props.active_connections)} max={500} color="bg-cyan-500" />
              )}
            </div>
          ) : (
            <p className="text-slate-500">No metrics available</p>
          )}
        </div>
      </div>

      {/* Agent Timeline */}
      <div className="bg-slate-900 border border-slate-700 rounded-xl p-6">
        <div className="flex items-center justify-between mb-4">
          <h3 className="text-sm font-semibold text-slate-300 uppercase">Agent Timeline — {alert.agent.name}</h3>
          <Link to={`/agents/${alert.agent.id}`} className="text-xs text-emerald-400 hover:text-emerald-300 transition-colors">
            View Agent Details →
          </Link>
        </div>
        <div className="space-y-1">
          {alert.agentTimeline.map((log) => {
            const isTriggering = log.id === alert.relatedLog.id;
            return (
              <div key={log.id} className={clsx(
                'flex items-center gap-3 px-3 py-2 rounded-lg text-sm',
                isTriggering ? 'bg-red-500/10 border border-red-500/20' : 'hover:bg-slate-800/50'
              )}>
                <div className="w-2 h-2 rounded-full bg-slate-600 flex-shrink-0" style={{
                  backgroundColor: isTriggering ? '#ef4444' : undefined
                }} />
                <span className="text-xs text-slate-500 font-mono w-20 flex-shrink-0">
                  {new Date(log.timestamp).toLocaleTimeString('tr-TR', { hour: '2-digit', minute: '2-digit', second: '2-digit' })}
                </span>
                <span className={clsx('px-1.5 py-0.5 rounded text-xs w-20 text-center flex-shrink-0', levelColors[log.level])}>
                  {log.level}
                </span>
                <span className="text-slate-400 text-xs w-28 flex-shrink-0">{log.source}</span>
                <span className="text-slate-300 truncate">{log.message}</span>
                {isTriggering && <span className="text-red-400 text-xs flex-shrink-0">← triggered alert</span>}
              </div>
            );
          })}
        </div>
      </div>
    </div>
  );
}
