using MediatR;

namespace Sentinel.Application.Commands.UpdateAgentStatus;

public record UpdateAgentStatusCommand(
    Guid AgentId,
    string Hostname,
    string DeviceType) : IRequest<Unit>;
