import { useQuery } from '@tanstack/react-query';
import { Server, Activity, AlertTriangle, ShieldAlert } from 'lucide-react';
import { getDashboardSummary, getAlerts } from '../api/client';
import SummaryCard from '../components/dashboard/SummaryCard';
import LogVolumeChart from '../components/dashboard/LogVolumeChart';
import AlertsByCategoryChart from '../components/dashboard/AlertsByCategoryChart';
import RecentAlertsTable from '../components/dashboard/RecentAlertsTable';

export default function DashboardPage() {
  const { data: summary } = useQuery({
    queryKey: ['dashboard'],
    queryFn: getDashboardSummary,
    refetchInterval: 10000,
  });

  const { data: alertsData } = useQuery({
    queryKey: ['alerts', 'recent'],
    queryFn: () => getAlerts({ pageSize: 5 }),
    refetchInterval: 10000,
  });

  if (!summary) {
    return <div className="text-slate-400 text-center mt-20">Loading dashboard...</div>;
  }

  return (
    <div className="space-y-6">
      <div>
        <h2 className="text-2xl font-bold text-white mb-1">Dashboard</h2>
        <p className="text-sm text-slate-400">Real-time system monitoring and threat detection</p>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
        <SummaryCard
          title="Total Agents"
          value={summary.totalAgents}
          subtitle={`${summary.onlineAgents} online`}
          icon={Server}
          color="blue"
        />
        <SummaryCard
          title="Total Logs"
          value={summary.totalLogs.toLocaleString()}
          subtitle={`${summary.criticalLogs24h} critical (24h)`}
          icon={Activity}
          color="emerald"
        />
        <SummaryCard
          title="Active Alerts"
          value={summary.unacknowledgedAlerts}
          subtitle={`${summary.totalAlerts} total`}
          icon={AlertTriangle}
          color="amber"
        />
        <SummaryCard
          title="Avg Risk Score"
          value={summary.averageRiskScore || '-'}
          subtitle="Last 24 hours"
          icon={ShieldAlert}
          color="red"
        />
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-4">
        <LogVolumeChart data={summary.logVolumeByHour} />
        <AlertsByCategoryChart data={summary.alertsByCategory} />
      </div>

      <RecentAlertsTable alerts={alertsData?.items ?? []} />
    </div>
  );
}
