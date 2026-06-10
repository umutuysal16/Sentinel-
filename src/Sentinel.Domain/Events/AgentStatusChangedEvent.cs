using Sentinel.Domain.Common;
using Sentinel.Domain.Enums;

namespace Sentinel.Domain.Events;

public record AgentStatusChangedEvent(
    Guid AgentId,
    AgentStatus OldStatus,
    AgentStatus NewStatus) : IDomainEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}
