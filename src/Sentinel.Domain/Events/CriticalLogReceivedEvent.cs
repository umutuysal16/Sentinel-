using Sentinel.Domain.Common;

namespace Sentinel.Domain.Events;

public record CriticalLogReceivedEvent(
    Guid LogEntryId,
    Guid AgentId,
    DateTime Timestamp) : IDomainEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}
