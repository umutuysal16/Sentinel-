using Sentinel.Domain.Common;

namespace Sentinel.Domain.Events;

public record AlertAcknowledgedEvent(
    Guid AlertId,
    string AcknowledgedBy) : IDomainEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}
