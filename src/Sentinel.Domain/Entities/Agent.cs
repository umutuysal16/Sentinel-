using Sentinel.Domain.Common;
using Sentinel.Domain.Enums;
using Sentinel.Domain.Events;

namespace Sentinel.Domain.Entities;

public class Agent : AggregateRoot
{
    public string Name { get; private set; } = null!;
    public string DeviceType { get; private set; } = null!;
    public AgentStatus Status { get; private set; }
    public DateTime LastHeartbeat { get; private set; }
    public string? IpAddress { get; private set; }
    public Dictionary<string, string> Metadata { get; private set; } = new();

    private readonly List<LogEntry> _logEntries = new();
    public IReadOnlyCollection<LogEntry> LogEntries => _logEntries.AsReadOnly();

    private Agent() { }

    public static Agent Create(string name, string deviceType, string? ipAddress = null)
    {
        var agent = new Agent
        {
            Name = name,
            DeviceType = deviceType,
            IpAddress = ipAddress,
            Status = AgentStatus.Online,
            LastHeartbeat = DateTime.UtcNow
        };

        return agent;
    }

    public void UpdateHeartbeat()
    {
        var oldStatus = Status;
        Status = AgentStatus.Online;
        LastHeartbeat = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        if (oldStatus != AgentStatus.Online)
        {
            AddDomainEvent(new AgentStatusChangedEvent(Id, oldStatus, AgentStatus.Online));
        }
    }

    public void MarkOffline()
    {
        if (Status == AgentStatus.Offline) return;

        var oldStatus = Status;
        Status = AgentStatus.Offline;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new AgentStatusChangedEvent(Id, oldStatus, AgentStatus.Offline));
    }

    public void MarkDegraded()
    {
        if (Status == AgentStatus.Degraded) return;

        var oldStatus = Status;
        Status = AgentStatus.Degraded;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new AgentStatusChangedEvent(Id, oldStatus, AgentStatus.Degraded));
    }
}
