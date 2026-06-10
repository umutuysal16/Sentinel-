using MediatR;

namespace Sentinel.Application.Commands.RegisterAgent;

public record RegisterAgentCommand(
    string Name,
    string DeviceType,
    string? IpAddress) : IRequest<Guid>;
