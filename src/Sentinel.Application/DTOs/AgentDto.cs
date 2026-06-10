namespace Sentinel.Application.DTOs;

public record AgentDto(
    Guid Id,
    string Name,
    string DeviceType,
    string Status,
    DateTime LastHeartbeat,
    string? IpAddress,
    int LogCount);
