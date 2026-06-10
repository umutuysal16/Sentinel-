using Sentinel.Application.DTOs;
using Sentinel.Domain.Entities;

namespace Sentinel.Application.Mappings;

public static class MappingExtensions
{
    public static LogEntryDto ToDto(this LogEntry logEntry) => new(
        logEntry.Id,
        logEntry.AgentId,
        logEntry.Agent?.Name ?? "Unknown",
        logEntry.Source,
        logEntry.Level.ToString(),
        logEntry.Message,
        logEntry.StackTrace,
        logEntry.Properties,
        logEntry.Timestamp);

    public static AgentDto ToDto(this Agent agent, int logCount = 0) => new(
        agent.Id,
        agent.Name,
        agent.DeviceType,
        agent.Status.ToString(),
        agent.LastHeartbeat,
        agent.IpAddress,
        logCount);

    public static AlertDto ToDto(this Alert alert) => new(
        alert.Id,
        alert.LogEntryId,
        alert.RiskScore,
        alert.Severity.ToString(),
        alert.Category.ToString(),
        alert.AiExplanation,
        alert.IsAcknowledged,
        alert.AcknowledgedAt,
        alert.AcknowledgedBy,
        alert.CreatedAt);
}
