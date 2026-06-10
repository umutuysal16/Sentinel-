using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Sentinel.Application.DTOs;
using Sentinel.Application.Interfaces;

namespace Sentinel.Infrastructure.Services;

public class SignalRNotificationService : IAlertNotificationService
{
    private readonly IHubContext<AlertHub> _hubContext;
    private readonly ILogger<SignalRNotificationService> _logger;

    public SignalRNotificationService(IHubContext<AlertHub> hubContext, ILogger<SignalRNotificationService> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task SendAlertAsync(AlertDto alert, CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.All.SendAsync("ReceiveAlert", alert, cancellationToken);
        _logger.LogInformation("Alert pushed via SignalR. AlertId: {AlertId}", alert.Id);
    }

    public async Task SendAgentStatusAsync(AgentDto agent, CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.All.SendAsync("ReceiveAgentStatus", agent, cancellationToken);
    }

    public async Task SendLogAsync(LogEntryDto log, CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.Group($"agent-{log.AgentId}").SendAsync("ReceiveLog", log, cancellationToken);
    }
}
