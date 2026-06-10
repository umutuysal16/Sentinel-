import axios from 'axios';
import type { Agent, Alert, AlertDetail, DashboardSummary, LogEntry, PaginatedList } from '../types';

const api = axios.create({
  baseURL: '/api',
});

// --- MOCK DATA FOR ACADEMIC SCREENSHOTS ---
const mockAgents: Agent[] = [
  { id: '1', name: 'Web-Server-01', ipAddress: '192.168.1.10', deviceType: 'Linux VM', osVersion: 'Ubuntu 22.04', status: 'Online', lastHeartbeat: new Date().toISOString(), logCount: 4521 },
  { id: '2', name: 'DB-Cluster-M', ipAddress: '10.0.0.5', deviceType: 'Database', osVersion: 'PostgreSQL', status: 'Online', lastHeartbeat: new Date().toISOString(), logCount: 12053 },
  { id: '3', name: 'Auth-Service', ipAddress: '172.16.0.4', deviceType: 'Container', osVersion: 'Docker', status: 'Degraded', lastHeartbeat: new Date().toISOString(), logCount: 890 },
];

const mockLogs: PaginatedList<LogEntry> = {
  items: [
    { id: 'l1', agentId: '1', agentName: 'Web-Server-01', level: 'Error', message: 'Failed to bind to port 8080. Address already in use.', source: 'nginx', timestamp: new Date(Date.now() - 5000).toISOString(), properties: null },
    { id: 'l2', agentId: '2', agentName: 'DB-Cluster-M', level: 'Information', message: 'Successfully committed transaction 89234', source: 'postgres', timestamp: new Date(Date.now() - 15000).toISOString(), properties: null },
    { id: 'l3', agentId: '3', agentName: 'Auth-Service', level: 'Critical', message: 'Multiple failed login attempts detected from IP 185.15.22.1', source: 'auth_middleware', timestamp: new Date(Date.now() - 25000).toISOString(), properties: null },
    { id: 'l4', agentId: '1', agentName: 'Web-Server-01', level: 'Warning', message: 'High memory usage detected (85%)', source: 'systemd', timestamp: new Date(Date.now() - 45000).toISOString(), properties: null },
    { id: 'l5', agentId: '2', agentName: 'DB-Cluster-M', level: 'Information', message: 'Vacuum analyze completed in 450ms', source: 'postgres', timestamp: new Date(Date.now() - 65000).toISOString(), properties: null },
  ],
  page: 1,
  pageSize: 50,
  totalCount: 5,
  totalPages: 1,
  hasNextPage: false,
  hasPreviousPage: false,
};

const mockAlerts: PaginatedList<Alert> = {
  items: [
    { id: 'a1', agentId: '3', agentName: 'Auth-Service', originalLogId: 'l3', ruleId: 'r1', severity: 'High', title: 'Potential Brute Force Attack', message: 'Detected 50+ failed logins in 1 minute. Initiating IP block.', isAcknowledged: false, createdAt: new Date(Date.now() - 25000).toISOString() },
    { id: 'a2', agentId: '1', agentName: 'Web-Server-01', originalLogId: 'l1', ruleId: 'r2', severity: 'Critical', title: 'Service Port Binding Failure', message: 'Critical web service failed to start due to port conflict.', isAcknowledged: false, createdAt: new Date(Date.now() - 5000).toISOString() },
  ],
  page: 1,
  pageSize: 50,
  totalCount: 2,
  totalPages: 1,
  hasNextPage: false,
  hasPreviousPage: false,
};

const mockSummary: DashboardSummary = {
  totalAgents: 3,
  activeAgents: 3,
  totalLogsToday: 17464,
  criticalAlertsActive: 2,
  recentLogs: mockLogs.items,
  recentAlerts: mockAlerts.items,
  logsByLevel: { 'Information': 12000, 'Warning': 3400, 'Error': 1900, 'Critical': 164 },
  logsByHour: { '08:00': 1500, '09:00': 2200, '10:00': 3500, '11:00': 2800, '12:00': 4100, '13:00': 3364 }
};

export const getAgents = () => Promise.resolve(mockAgents);
export const getAlerts = () => Promise.resolve(mockAlerts);
export const getLogs = () => Promise.resolve(mockLogs);
export const getDashboardSummary = () => Promise.resolve(mockSummary);
export const acknowledgeAlert = (id: string) => Promise.resolve();
export const getLogById = (id: string) => Promise.resolve(mockLogs.items[0]);
export const getAlertById = (id: string) => Promise.resolve({ ...mockAlerts.items[0], ruleName: 'Security Policy', ruleDescription: 'Blocks repeated auth failures' } as AlertDetail);
export const getAlertStats = () => Promise.resolve({ total: 15, unacknowledged: 2, bySeverity: { 'Critical': 1, 'High': 1, 'Medium': 8, 'Low': 5 } });
export const clearAlerts = () => Promise.resolve();
