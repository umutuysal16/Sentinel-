import { BarChart, Bar, XAxis, YAxis, Tooltip, ResponsiveContainer, Cell } from 'recharts';

interface Props {
  data: Record<string, number>;
}

const COLORS: Record<string, string> = {
  BruteForce: '#ef4444',
  UnauthorizedAccess: '#f97316',
  ResourceBottleneck: '#eab308',
  AnomalousPattern: '#a855f7',
  ServiceDown: '#ec4899',
};

export default function AlertsByCategoryChart({ data }: Props) {
  const chartData = Object.entries(data).map(([category, count]) => ({
    category,
    count,
  }));

  if (chartData.length === 0) {
    return (
      <div className="bg-slate-900 border border-slate-700 rounded-xl p-5">
        <h3 className="text-sm font-medium text-slate-400 mb-4">Alerts by Category</h3>
        <div className="flex items-center justify-center h-[250px] text-slate-500">
          No alerts yet
        </div>
      </div>
    );
  }

  return (
    <div className="bg-slate-900 border border-slate-700 rounded-xl p-5">
      <h3 className="text-sm font-medium text-slate-400 mb-4">Alerts by Category</h3>
      <ResponsiveContainer width="100%" height={250}>
        <BarChart data={chartData}>
          <XAxis dataKey="category" stroke="#64748b" fontSize={11} angle={-20} textAnchor="end" height={60} />
          <YAxis stroke="#64748b" fontSize={12} allowDecimals={false} />
          <Tooltip
            contentStyle={{ backgroundColor: '#1e293b', border: '1px solid #334155', borderRadius: '8px', color: '#e2e8f0' }}
          />
          <Bar dataKey="count" radius={[4, 4, 0, 0]}>
            {chartData.map((entry) => (
              <Cell key={entry.category} fill={COLORS[entry.category] || '#6366f1'} />
            ))}
          </Bar>
        </BarChart>
      </ResponsiveContainer>
    </div>
  );
}
