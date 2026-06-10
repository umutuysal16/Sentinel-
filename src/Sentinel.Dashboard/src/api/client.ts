import axios from 'axios';
import type { Agent, Alert, AlertDetail, DashboardSummary, LogEntry, PaginatedList } from '../types';

const api = axios.create({
  baseURL: '/api',
});

export const getAgents = () =>
  api.get<Agent[]>('/agents').then(r => r.data);

export const getAlerts = (params?: { severity?: string; acknowledged?: boolean; page?: number; pageSize?: number }) =>
  api.get<PaginatedList<Alert>>('/alerts', { params }).then(r => r.data);

export const getLogs = (params?: { agentId?: string; level?: number; page?: number; pageSize?: number }) =>
  api.get<PaginatedList<LogEntry>>('/logs', { params }).then(r => r.data);

export const getDashboardSummary = () =>
  api.get<DashboardSummary>('/dashboard/summary').then(r => r.data);

export const acknowledgeAlert = (id: string) =>
  api.post(`/alerts/${id}/acknowledge`).then(r => r.data);

export const getLogById = (id: string) =>
  api.get<LogEntry>(`/logs/${id}`).then(r => r.data);

export const getAlertById = (id: string) =>
  api.get<AlertDetail>(`/alerts/${id}`).then(r => r.data);

export const getAlertStats = () =>
  api.get('/alerts/stats').then(r => r.data);

export const clearAlerts = (onlyAcknowledged = true) =>
  api.delete('/alerts', { params: { onlyAcknowledged } }).then(r => r.data);
