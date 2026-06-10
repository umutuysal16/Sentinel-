import type { Alert } from '../../types';
import clsx from 'clsx';

interface Props {
  alerts: Alert[];
}

const severityColor: Record<string, string> = {
  Low: 'bg-blue-500/20 text-blue-400',
  Medium: 'bg-amber-500/20 text-amber-400',
  High: 'bg-orange-500/20 text-orange-400',
  Critical: 'bg-red-500/20 text-red-400',
};

export default function RecentAlertsTable({ alerts }: Props) {
  if (alerts.length === 0) {
    return (
      <div className="bg-slate-900 border border-slate-700 rounded-xl p-5">
        <h3 className="text-sm font-medium text-slate-400 mb-4">Recent Alerts</h3>
        <div className="text-slate-500 text-center py-8">No alerts yet</div>
      </div>
    );
  }

  return (
    <div className="bg-slate-900 border border-slate-700 rounded-xl p-5">
      <h3 className="text-sm font-medium text-slate-400 mb-4">Recent Alerts</h3>
      <div className="space-y-3">
        {alerts.map(alert => (
          <div key={alert.id} className="flex items-center gap-4 bg-slate-800/50 rounded-lg p-3">
            <div className="text-2xl font-bold text-white w-12 text-center">
              {alert.riskScore}
            </div>
            <div className="flex-1 min-w-0">
              <div className="flex items-center gap-2 mb-1">
                <span className={clsx('px-2 py-0.5 rounded text-xs font-medium', severityColor[alert.severity])}>
                  {alert.severity}
                </span>
                <span className="text-xs text-slate-500">{alert.category}</span>
              </div>
              <p className="text-sm text-slate-300 truncate">{alert.aiExplanation}</p>
            </div>
            <div className="text-xs text-slate-500 whitespace-nowrap">
              {new Date(alert.createdAt).toLocaleTimeString('tr-TR')}
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}
