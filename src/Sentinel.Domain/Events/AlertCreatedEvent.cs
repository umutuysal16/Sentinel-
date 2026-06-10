using Sentinel.Domain.Common;
using Sentinel.Domain.Enums;

namespace Sentinel.Domain.Events;

public record AlertCreatedEvent(
    Guid AlertId,
    int RiskScore,
    AlertSeverity Severity,
    AlertCategory Category) : IDomainEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}
