import { Outlet } from 'react-router-dom';
import Sidebar from './Sidebar';
import { useSignalR } from '../../hooks/useSignalR';

export default function AppLayout() {
  useSignalR();

  return (
    <div className="flex min-h-screen bg-slate-950">
      <Sidebar />
      <main className="flex-1 p-6 overflow-auto">
        <Outlet />
      </main>
    </div>
  );
}
