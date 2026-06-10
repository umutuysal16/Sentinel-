import { NavLink } from 'react-router-dom';
import { LayoutDashboard, ScrollText, AlertTriangle, Server, Shield, Sun, Moon, Volume2, VolumeX } from 'lucide-react';
import clsx from 'clsx';
import { useTheme } from '../../context/ThemeContext';
import { useAlertSound } from '../../hooks/useAlertSound';

const links = [
  { to: '/', icon: LayoutDashboard, label: 'Dashboard' },
  { to: '/logs', icon: ScrollText, label: 'Logs' },
  { to: '/alerts', icon: AlertTriangle, label: 'Alerts' },
  { to: '/agents', icon: Server, label: 'Agents' },
];

export default function Sidebar() {
  const { theme, toggleTheme } = useTheme();
  const { soundEnabled, toggleSound } = useAlertSound();

  return (
    <aside className="w-64 bg-slate-900 border-r border-slate-700 flex flex-col min-h-screen">
      <div className="p-6 border-b border-slate-700">
        <div className="flex items-center gap-3">
          <Shield className="w-8 h-8 text-emerald-400" />
          <div>
            <h1 className="text-lg font-bold text-white">Sentinel</h1>
            <p className="text-xs text-slate-400">AI Log Analysis</p>
          </div>
        </div>
      </div>

      <nav className="flex-1 p-4 space-y-1">
        {links.map(({ to, icon: Icon, label }) => (
          <NavLink
            key={to}
            to={to}
            end={to === '/'}
            className={({ isActive }) =>
              clsx(
                'flex items-center gap-3 px-4 py-3 rounded-lg text-sm font-medium transition-colors',
                isActive
                  ? 'bg-emerald-500/10 text-emerald-400 border border-emerald-500/20'
                  : 'text-slate-400 hover:bg-slate-800 hover:text-slate-200'
              )
            }
          >
            <Icon className="w-5 h-5" />
            {label}
          </NavLink>
        ))}
      </nav>

      <div className="p-4 border-t border-slate-700 space-y-3">
        <div className="flex items-center justify-center gap-2">
          <button
            onClick={toggleTheme}
            className="flex items-center gap-2 px-3 py-2 rounded-lg text-xs text-slate-400 hover:bg-slate-800 hover:text-slate-200 transition-colors"
            title={theme === 'dark' ? 'Switch to light theme' : 'Switch to dark theme'}
          >
            {theme === 'dark' ? <Sun className="w-4 h-4" /> : <Moon className="w-4 h-4" />}
            {theme === 'dark' ? 'Light' : 'Dark'}
          </button>
          <button
            onClick={toggleSound}
            className="flex items-center gap-2 px-3 py-2 rounded-lg text-xs text-slate-400 hover:bg-slate-800 hover:text-slate-200 transition-colors"
            title={soundEnabled ? 'Mute alerts' : 'Enable alert sounds'}
          >
            {soundEnabled ? <Volume2 className="w-4 h-4" /> : <VolumeX className="w-4 h-4" />}
            {soundEnabled ? 'Sound' : 'Muted'}
          </button>
        </div>
        <p className="text-xs text-slate-500 text-center">Sentinel v1.0</p>
      </div>
    </aside>
  );
}
