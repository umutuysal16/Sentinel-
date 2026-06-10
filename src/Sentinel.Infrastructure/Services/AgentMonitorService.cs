using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sentinel.Domain.Enums;
using Sentinel.Infrastructure.Persistence;

namespace Sentinel.Infrastructure.Services;

public class AgentMonitorService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AgentMonitorService> _logger;

    public AgentMonitorService(IServiceScopeFactory scopeFactory, ILogger<AgentMonitorService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<SentinelDbContext>();

                var threshold = DateTime.UtcNow.AddMinutes(-2);

                var staleAgents = await dbContext.Agents
                    .Where(a => a.Status != AgentStatus.Offline && a.LastHeartbeat < threshold)
                    .ToListAsync(stoppingToken);

                foreach (var agent in staleAgents)
                {
                    agent.MarkOffline();
                    _logger.LogWarning("Agent {AgentName} marked as Offline. Last heartbeat: {LastHeartbeat}",
                        agent.Name, agent.LastHeartbeat);
                }

                if (staleAgents.Count > 0)
                    await dbContext.SaveChangesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AgentMonitorService.");
            }

            await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
        }
    }
}
