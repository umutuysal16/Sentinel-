using Sentinel.Application.DTOs;

namespace Sentinel.Application.Interfaces;

public interface IAlertNotificationService
{
    Task SendAlertAsync(AlertDto alert, CancellationToken cancellationToken = default);
    Task SendAgentStatusAsync(AgentDto agent, CancellationToken cancellationToken = default);
    Task SendLogAsync(LogEntryDto log, CancellationToken cancellationToken = default);
}
