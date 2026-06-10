using MediatR;
using Sentinel.Domain.Common;

namespace Sentinel.Application.EventHandlers;

public record DomainEventNotification<TDomainEvent>(TDomainEvent DomainEvent) : INotification
    where TDomainEvent : IDomainEvent;
