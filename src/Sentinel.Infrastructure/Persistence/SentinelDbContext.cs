using MediatR;
using Microsoft.EntityFrameworkCore;
using Sentinel.Application.EventHandlers;
using Sentinel.Application.Interfaces;
using Sentinel.Domain.Common;
using Sentinel.Domain.Entities;

namespace Sentinel.Infrastructure.Persistence;

public class SentinelDbContext : DbContext, IApplicationDbContext
{
    private readonly IMediator _mediator;

    public DbSet<Agent> Agents => Set<Agent>();
    public DbSet<LogEntry> LogEntries => Set<LogEntry>();
    public DbSet<Alert> Alerts => Set<Alert>();

    public SentinelDbContext(DbContextOptions<SentinelDbContext> options, IMediator mediator)
        : base(options)
    {
        _mediator = mediator;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SentinelDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Collect domain events before saving
        var aggregateRoots = ChangeTracker
            .Entries<AggregateRoot>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity)
            .ToList();

        var domainEvents = aggregateRoots
            .SelectMany(ar => ar.DomainEvents)
            .ToList();

        // Clear events before saving to prevent re-dispatch
        aggregateRoots.ForEach(ar => ar.ClearDomainEvents());

        // Set audit timestamps
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Modified)
                entry.Entity.GetType().GetProperty("UpdatedAt")?.SetValue(entry.Entity, DateTime.UtcNow);
        }

        // Persist data first
        var result = await base.SaveChangesAsync(cancellationToken);

        // Dispatch domain events after successful save
        foreach (var domainEvent in domainEvents)
        {
            var notificationType = typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType());
            var notification = Activator.CreateInstance(notificationType, domainEvent);
            if (notification is not null)
                await _mediator.Publish(notification, cancellationToken);
        }

        return result;
    }
}
