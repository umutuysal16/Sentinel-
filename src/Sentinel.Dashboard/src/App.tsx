import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { ThemeProvider } from './context/ThemeContext';
import AppLayout from './components/layout/AppLayout';
import DashboardPage from './pages/DashboardPage';
import LogsPage from './pages/LogsPage';
import AlertsPage from './pages/AlertsPage';
import AgentsPage from './pages/AgentsPage';
import AgentDetailPage from './pages/AgentDetailPage';
import AlertDetailPage from './pages/AlertDetailPage';

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 5000,
      retry: 1,
    },
  },
});

export default function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <ThemeProvider>
        <BrowserRouter>
          <Routes>
            <Route element={<AppLayout />}>
              <Route path="/" element={<DashboardPage />} />
              <Route path="/logs" element={<LogsPage />} />
              <Route path="/alerts" element={<AlertsPage />} />
              <Route path="/alerts/:id" element={<AlertDetailPage />} />
              <Route path="/agents" element={<AgentsPage />} />
              <Route path="/agents/:id" element={<AgentDetailPage />} />
            </Route>
          </Routes>
        </BrowserRouter>
      </ThemeProvider>
    </QueryClientProvider>
  );
}
