import { AreaChart, Area, XAxis, YAxis, Tooltip, ResponsiveContainer } from 'recharts';

interface Props {
  data: { hour: string; count: number }[];
}

export default function LogVolumeChart({ data }: Props) {
  const chartData = data.map(d => ({
    time: new Date(d.hour).toLocaleTimeString('tr-TR', { hour: '2-digit', minute: '2-digit' }),
    count: d.count,
  }));

  return (
    <div className="bg-slate-900 border border-slate-700 rounded-xl p-5">
      <h3 className="text-sm font-medium text-slate-400 mb-4">Log Volume (Last 24h)</h3>
      <ResponsiveContainer width="100%" height={250}>
        <AreaChart data={chartData}>
          <defs>
            <linearGradient id="logGradient" x1="0" y1="0" x2="0" y2="1">
              <stop offset="5%" stopColor="#10b981" stopOpacity={0.3} />
              <stop offset="95%" stopColor="#10b981" stopOpacity={0} />
            </linearGradient>
          </defs>
          <XAxis dataKey="time" stroke="#64748b" fontSize={12} />
          <YAxis stroke="#64748b" fontSize={12} />
          <Tooltip
            contentStyle={{ backgroundColor: '#1e293b', border: '1px solid #334155', borderRadius: '8px', color: '#e2e8f0' }}
          />
          <Area type="monotone" dataKey="count" stroke="#10b981" fill="url(#logGradient)" strokeWidth={2} />
        </AreaChart>
      </ResponsiveContainer>
    </div>
  );
}
