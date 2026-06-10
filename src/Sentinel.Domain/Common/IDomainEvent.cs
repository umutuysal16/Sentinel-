namespace Sentinel.Domain.Common;

public interface IDomainEvent
{
    DateTime OccurredAt { get; }
}
