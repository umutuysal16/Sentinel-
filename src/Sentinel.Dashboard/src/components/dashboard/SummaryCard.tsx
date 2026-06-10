import type { LucideIcon } from 'lucide-react';
import clsx from 'clsx';

interface Props {
  title: string;
  value: number | string;
  icon: LucideIcon;
  color: 'emerald' | 'amber' | 'red' | 'blue';
  subtitle?: string;
}

const colorMap = {
  emerald: 'bg-emerald-500/10 text-emerald-400 border-emerald-500/20',
  amber: 'bg-amber-500/10 text-amber-400 border-amber-500/20',
  red: 'bg-red-500/10 text-red-400 border-red-500/20',
  blue: 'bg-blue-500/10 text-blue-400 border-blue-500/20',
};

const iconColorMap = {
  emerald: 'text-emerald-400',
  amber: 'text-amber-400',
  red: 'text-red-400',
  blue: 'text-blue-400',
};

export default function SummaryCard({ title, value, icon: Icon, color, subtitle }: Props) {
  return (
    <div className={clsx('rounded-xl border p-5', colorMap[color])}>
      <div className="flex items-center justify-between">
        <div>
          <p className="text-sm text-slate-400 mb-1">{title}</p>
          <p className="text-3xl font-bold text-white">{value}</p>
          {subtitle && <p className="text-xs text-slate-500 mt-1">{subtitle}</p>}
        </div>
        <Icon className={clsx('w-10 h-10 opacity-60', iconColorMap[color])} />
      </div>
    </div>
  );
}
