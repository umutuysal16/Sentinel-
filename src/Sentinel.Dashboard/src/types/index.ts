export interface LogEntry {
  id: string;
  agentId: string;
  agentName: string;
  source: string;
  level: string;
  message: string;
  stackTrace: string | null;
  properties: Record<string, number>;
  timestamp: string;
}

export interface Agent {
  id: string;
  name: string;
  deviceType: string;
  status: string;
  lastHeartbeat: string;
  ipAddress: string | null;
  logCount: number;
}

export interface Alert {
  id: string;
  logEntryId: string;
  riskScore: number;
  severity: string;
  category: string;
  aiExplanation: string;
  isAcknowledged: boolean;
  acknowledgedAt: string | null;
  acknowledgedBy: string | null;
  createdAt: string;
}

export interface DashboardSummary {
  totalAgents: number;
  onlineAgents: number;
  totalLogs: number;
  criticalLogs24h: number;
  totalAlerts: number;
  unacknowledgedAlerts: number;
  averageRiskScore: number;
  alertsByCategory: Record<string, number>;
  logVolumeByHour: { hour: string; count: number }[];
}

export interface AlertDetail {
  id: string;
  riskScore: number;
  severity: string;
  category: string;
  aiExplanation: string;
  isAcknowledged: boolean;
  acknowledgedAt: string | null;
  acknowledgedBy: string | null;
  createdAt: string;
  relatedLog: LogEntry;
  agent: Agent;
  agentTimeline: LogEntry[];
}

export interface PaginatedList<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}
