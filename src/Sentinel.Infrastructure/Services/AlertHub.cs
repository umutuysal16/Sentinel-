using Microsoft.AspNetCore.SignalR;

namespace Sentinel.Infrastructure.Services;

public class AlertHub : Hub
{
    public async Task JoinAgentGroup(string agentId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"agent-{agentId}");
    }

    public async Task LeaveAgentGroup(string agentId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"agent-{agentId}");
    }
}
