import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { Link } from 'react-router-dom';
import { getAlerts, acknowledgeAlert } from '../api/client';
import clsx from 'clsx';
import { CheckCircle, Clock, ExternalLink } from 'lucide-react';

const severityColor: Record<string, string> = {
  Low: 'bg-blue-500/20 text-blue-400 border-blue-500/30',
  Medium: 'bg-amber-500/20 text-amber-400 border-amber-500/30',
  High: 'bg-orange-500/20 text-orange-400 border-orange-500/30',
  Critical: 'bg-red-500/20 text-red-400 border-red-500/30',
};

const riskColor = (score: number) => {
  if (score <= 3) return 'text-blue-400';
  if (score <= 5) return 'text-amber-400';
  if (score <= 8) return 'text-orange-400';
  return 'text-red-400';
};

export default function AlertsPage() {
  const [severity, setSeverity] = useState('');
  const [acknowledged, setAcknowledged] = useState<boolean | undefined>(undefined);
  const [page, setPage] = useState(1);
  const queryClient = useQueryClient();

  const { data, isLoading } = useQuery({
    queryKey: ['alerts', severity, acknowledged, page],
    queryFn: () => getAlerts({
      severity: severity || undefined,
      acknowledged,
      page,
      pageSize: 20,
    }),
    refetchInterval: 10000,
  });

  const ackMutation = useMutation({
    mutationFn: acknowledgeAlert,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['alerts'] });
      queryClient.invalidateQueries({ queryKey: ['dashboard'] });
    },
  });

  return (
    <div className="space-y-6">
      <div>
        <h2 className="text-2xl font-bold text-white mb-1">Security Alerts</h2>
        <p className="text-sm text-slate-400">AI-powered threat detection results</p>
      </div>

      {/* Filters */}
      <div className="flex gap-4 flex-wrap">
        <select
          value={severity}
          onChange={e => { setSeverity(e.target.value); setPage(1); }}
          className="bg-slate-800 border border-slate-700 rounded-lg px-3 py-2 text-sm text-slate-200"
        >
          <option value="">All Severities</option>
          <option value="Low">Low</option>
          <option value="Medium">Medium</option>
          <option value="High">High</option>
          <option value="Critical">Critical</option>
        </select>

        <select
          value={acknowledged === undefined ? '' : String(acknowledged)}
          onChange={e => {
            setAcknowledged(e.target.value === '' ? undefined : e.target.value === 'true');
            setPage(1);
          }}
          className="bg-slate-800 border border-slate-700 rounded-lg px-3 py-2 text-sm text-slate-200"
        >
          <option value="">All Status</option>
          <option value="false">Unacknowledged</option>
          <option value="true">Acknowledged</option>
        </select>
      </div>

      {/* Alerts Grid */}
      {isLoading ? (
        <div className="text-slate-500 text-center py-8">Loading...</div>
      ) : data?.items.length === 0 ? (
        <div className="bg-slate-900 border border-slate-700 rounded-xl p-12 text-center">
          <p className="text-slate-500">No alerts found. The system is running smoothly.</p>
        </div>
      ) : (
        <div className="grid gap-4">
          {data?.items.map(alert => (
            <div key={alert.id} className={clsx('bg-slate-900 border rounded-xl p-5', severityColor[alert.severity])}>
              <div className="flex items-start gap-5">
                <div className={clsx('text-4xl font-bold w-16 text-center', riskColor(alert.riskScore))}>
                  {alert.riskScore}
                </div>
                <div className="flex-1 min-w-0">
                  <div className="flex items-center gap-2 mb-2">
                    <span className={clsx('px-2 py-0.5 rounded text-xs font-medium', severityColor[alert.severity])}>
                      {alert.severity}
                    </span>
                    <span className="text-xs text-slate-500 bg-slate-800 px-2 py-0.5 rounded">{alert.category}</span>
                    <span className="text-xs text-slate-500 flex items-center gap-1">
                      <Clock className="w-3 h-3" />
                      {new Date(alert.createdAt).toLocaleString('tr-TR')}
                    </span>
                  </div>
                  <p className="text-sm text-slate-200 mb-2">{alert.aiExplanation}</p>
                  <div className="flex items-center gap-3">
                    <p className="text-xs text-slate-500">Log Entry: {alert.logEntryId.slice(0, 8)}...</p>
                    <Link to={`/alerts/${alert.id}`} className="text-xs text-emerald-400 hover:text-emerald-300 flex items-center gap-1 transition-colors">
                      <ExternalLink className="w-3 h-3" /> View Details
                    </Link>
                  </div>
                </div>
                <div>
                  {alert.isAcknowledged ? (
                    <span className="flex items-center gap-1 text-emerald-400 text-xs">
                      <CheckCircle className="w-4 h-4" /> Acknowledged
                    </span>
                  ) : (
                    <button
                      onClick={() => ackMutation.mutate(alert.id)}
                      disabled={ackMutation.isPending}
                      className="px-4 py-2 bg-emerald-600 hover:bg-emerald-500 text-white text-sm rounded-lg transition-colors disabled:opacity-50"
                    >
                      Acknowledge
                    </button>
                  )}
                </div>
              </div>
            </div>
          ))}
        </div>
      )}

      {/* Pagination */}
      {data && data.totalPages > 1 && (
        <div className="flex items-center justify-between">
          <p className="text-sm text-slate-500">Page {data.page} of {data.totalPages}</p>
          <div className="flex gap-2">
            <button onClick={() => setPage(p => p - 1)} disabled={!data.hasPreviousPage}
              className="px-3 py-1.5 bg-slate-800 border border-slate-700 rounded text-sm text-slate-300 disabled:opacity-30">
              Previous
            </button>
            <button onClick={() => setPage(p => p + 1)} disabled={!data.hasNextPage}
              className="px-3 py-1.5 bg-slate-800 border border-slate-700 rounded text-sm text-slate-300 disabled:opacity-30">
              Next
            </button>
          </div>
        </div>
      )}
    </div>
  );
}
