import { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { getLogs, getAgents } from '../api/client';
import clsx from 'clsx';

const levelColors: Record<string, string> = {
  Debug: 'bg-slate-500/20 text-slate-400',
  Information: 'bg-blue-500/20 text-blue-400',
  Warning: 'bg-amber-500/20 text-amber-400',
  Error: 'bg-red-500/20 text-red-400',
  Critical: 'bg-red-600/30 text-red-300 font-bold',
};

export default function LogsPage() {
  const [agentId, setAgentId] = useState<string>('');
  const [level, setLevel] = useState<number | undefined>(undefined);
  const [page, setPage] = useState(1);

  const { data: agents } = useQuery({
    queryKey: ['agents'],
    queryFn: getAgents,
  });

  const { data, isLoading } = useQuery({
    queryKey: ['logs', agentId, level, page],
    queryFn: () => getLogs({
      agentId: agentId || undefined,
      level,
      page,
      pageSize: 30,
    }),
    refetchInterval: 5000,
  });

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-start">
        <div>
          <h2 className="text-2xl font-bold text-white mb-1">System Logs</h2>
          <p className="text-sm text-slate-400">Real-time log stream from all agents</p>
        </div>
        <button
          onClick={async () => {
            try {
              // 1. Register agent
              const regRes = await fetch('/api/BrowserLogs/register', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ Name: 'Chrome-Security-Extension-V1', IpAddress: '127.0.0.1' })
              });
              
              if (!regRes.ok) {
                  throw new Error(`Register failed: ${regRes.statusText}`);
              }
              const { agentId } = await regRes.json();
              
              // 2. Send Critical Log
              const logRes = await fetch('/api/BrowserLogs', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                  AgentId: agentId,
                  Level: 'Critical',
                  Message: 'XSS Payload Detected: <script>alert(1)</script>',
                  Source: 'SecurityScanner',
                  Properties: { Url: window.location.href }
                })
              });
              
              if (logRes.ok) {
                  alert('Saldırı simülasyonu başarıyla API\'ye gönderildi! Ekrana düşmesi için birkaç saniye bekle.');
              }
            } catch (err) {
              console.error(err);
              alert('Bir hata oluştu: ' + err);
            }
          }}
          className="bg-red-600 hover:bg-red-700 text-white px-4 py-2 rounded-lg text-sm font-medium transition-colors shadow-lg shadow-red-900/20 flex items-center gap-2"
        >
          🚨 Simulate XSS Attack (Plugin)
        </button>
      </div>

      {/* Filters */}
      <div className="flex gap-4 flex-wrap">
        <select
          value={agentId}
          onChange={e => { setAgentId(e.target.value); setPage(1); }}
          className="bg-slate-800 border border-slate-700 rounded-lg px-3 py-2 text-sm text-slate-200"
        >
          <option value="">All Agents</option>
          {agents?.map(a => (
            <option key={a.id} value={a.id}>{a.name}</option>
          ))}
        </select>

        <select
          value={level ?? ''}
          onChange={e => { setLevel(e.target.value ? Number(e.target.value) : undefined); setPage(1); }}
          className="bg-slate-800 border border-slate-700 rounded-lg px-3 py-2 text-sm text-slate-200"
        >
          <option value="">All Levels</option>
          <option value="0">Debug</option>
          <option value="1">Information</option>
          <option value="2">Warning</option>
          <option value="3">Error</option>
          <option value="4">Critical</option>
        </select>
      </div>

      {/* Log Table */}
      <div className="bg-slate-900 border border-slate-700 rounded-xl overflow-hidden">
        <table className="w-full text-sm">
          <thead>
            <tr className="border-b border-slate-700 text-slate-400">
              <th className="text-left px-4 py-3 w-24">Level</th>
              <th className="text-left px-4 py-3 w-36">Time</th>
              <th className="text-left px-4 py-3 w-32">Agent</th>
              <th className="text-left px-4 py-3 w-36">Source</th>
              <th className="text-left px-4 py-3">Message</th>
            </tr>
          </thead>
          <tbody>
            {isLoading ? (
              <tr><td colSpan={5} className="text-center py-8 text-slate-500">Loading...</td></tr>
            ) : data?.items.map(log => (
              <tr key={log.id} className="border-b border-slate-800 hover:bg-slate-800/50 transition-colors">
                <td className="px-4 py-2">
                  <span className={clsx('px-2 py-0.5 rounded text-xs font-medium', levelColors[log.level])}>
                    {log.level}
                  </span>
                </td>
                <td className="px-4 py-2 text-slate-400 text-xs font-mono">
                  {new Date(log.timestamp).toLocaleTimeString('tr-TR', { hour: '2-digit', minute: '2-digit', second: '2-digit' })}
                </td>
                <td className="px-4 py-2 text-slate-300">{log.agentName}</td>
                <td className="px-4 py-2 text-slate-400">{log.source}</td>
                <td className="px-4 py-2 text-slate-200 truncate max-w-md">{log.message}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {/* Pagination */}
      {data && data.totalPages > 1 && (
        <div className="flex items-center justify-between">
          <p className="text-sm text-slate-500">
            Page {data.page} of {data.totalPages} ({data.totalCount} total)
          </p>
          <div className="flex gap-2">
            <button
              onClick={() => setPage(p => p - 1)}
              disabled={!data.hasPreviousPage}
              className="px-3 py-1.5 bg-slate-800 border border-slate-700 rounded text-sm text-slate-300 disabled:opacity-30"
            >
              Previous
            </button>
            <button
              onClick={() => setPage(p => p + 1)}
              disabled={!data.hasNextPage}
              className="px-3 py-1.5 bg-slate-800 border border-slate-700 rounded text-sm text-slate-300 disabled:opacity-30"
            >
              Next
            </button>
          </div>
        </div>
      )}
    </div>
  );
}
