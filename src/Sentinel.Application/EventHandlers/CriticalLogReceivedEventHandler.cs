using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sentinel.Application.Commands.CreateAlert;
using Sentinel.Application.Interfaces;
using Sentinel.Domain.Events;

namespace Sentinel.Application.EventHandlers;

public class CriticalLogReceivedEventHandler : INotificationHandler<DomainEventNotification<CriticalLogReceivedEvent>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IAiAnalysisService _aiAnalysisService;
    private readonly IMediator _mediator;
    private readonly ILogger<CriticalLogReceivedEventHandler> _logger;

    public CriticalLogReceivedEventHandler(
        IApplicationDbContext dbContext,
        IAiAnalysisService aiAnalysisService,
        IMediator mediator,
        ILogger<CriticalLogReceivedEventHandler> logger)
    {
        _dbContext = dbContext;
        _aiAnalysisService = aiAnalysisService;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Handle(DomainEventNotification<CriticalLogReceivedEvent> notification, CancellationToken cancellationToken)
    {
        var evt = notification.DomainEvent;

        _logger.LogWarning("Critical log received. LogEntryId: {LogEntryId}, AgentId: {AgentId}",
            evt.LogEntryId, evt.AgentId);

        var logEntry = await _dbContext.LogEntries
            .FirstOrDefaultAsync(l => l.Id == evt.LogEntryId, cancellationToken);

        if (logEntry is null)
        {
            _logger.LogError("LogEntry {LogEntryId} not found for AI analysis.", evt.LogEntryId);
            return;
        }

        var result = await _aiAnalysisService.AnalyzeLogAsync(
            logEntry.Id,
            logEntry.Message,
            logEntry.Source,
            logEntry.Level.ToString(),
            cancellationToken);

        if (!result.Success)
        {
            _logger.LogWarning("AI analysis failed for LogEntry {LogEntryId}. System continues without alert.", evt.LogEntryId);
            return;
        }

        if (result.RiskScore >= 1)
        {
            await _mediator.Send(new CreateAlertCommand(
                logEntry.Id,
                result.RiskScore,
                result.Category,
                result.Explanation), cancellationToken);

            _logger.LogInformation("Alert created for LogEntry {LogEntryId}. RiskScore: {RiskScore}, Category: {Category}",
                evt.LogEntryId, result.RiskScore, result.Category);
        }
    }
}
