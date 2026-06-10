using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sentinel.Application.Interfaces;
using Sentinel.Application.Mappings;
using Sentinel.Domain.Events;

namespace Sentinel.Application.EventHandlers;

public class AlertCreatedEventHandler : INotificationHandler<DomainEventNotification<AlertCreatedEvent>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IAlertNotificationService _notificationService;
    private readonly ILogger<AlertCreatedEventHandler> _logger;

    public AlertCreatedEventHandler(
        IApplicationDbContext dbContext,
        IAlertNotificationService notificationService,
        ILogger<AlertCreatedEventHandler> logger)
    {
        _dbContext = dbContext;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task Handle(DomainEventNotification<AlertCreatedEvent> notification, CancellationToken cancellationToken)
    {
        var evt = notification.DomainEvent;

        var alert = await _dbContext.Alerts
            .FirstOrDefaultAsync(a => a.Id == evt.AlertId, cancellationToken);

        if (alert is null) return;

        var alertDto = alert.ToDto();
        await _notificationService.SendAlertAsync(alertDto, cancellationToken);

        _logger.LogInformation("Alert pushed via SignalR. AlertId: {AlertId}, Severity: {Severity}",
            evt.AlertId, evt.Severity);
    }
}
