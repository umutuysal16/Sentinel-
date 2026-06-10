using Sentinel.Domain.Common;
using Sentinel.Domain.Enums;
using Sentinel.Domain.Events;

namespace Sentinel.Domain.Entities;

public class LogEntry : AggregateRoot
{
    public Guid AgentId { get; private set; }
    public string Source { get; private set; } = null!;
    public LogLevel Level { get; private set; }
    public string Message { get; private set; } = null!;
    public string? StackTrace { get; private set; }
    public Dictionary<string, object> Properties { get; private set; } = new();
    public DateTime Timestamp { get; private set; }

    public Agent Agent { get; private set; } = null!;

    private LogEntry() { }

    public static LogEntry Create(
        Guid agentId,
        string source,
        LogLevel level,
        string message,
        DateTime timestamp,
        string? stackTrace = null,
        Dictionary<string, object>? properties = null)
    {
        if (agentId == Guid.Empty)
            throw new ArgumentException("AgentId cannot be empty.", nameof(agentId));

        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Message cannot be empty.", nameof(message));

        var logEntry = new LogEntry
        {
            AgentId = agentId,
            Source = source,
            Level = level,
            Message = message,
            Timestamp = timestamp,
            StackTrace = stackTrace,
            Properties = properties ?? new Dictionary<string, object>()
        };

        if (logEntry.IsCritical())
        {
            logEntry.AddDomainEvent(new CriticalLogReceivedEvent(
                logEntry.Id,
                logEntry.AgentId,
                logEntry.Timestamp));
        }

        return logEntry;
    }

    public bool IsCritical() => Level >= LogLevel.Error;
}
