using System.ComponentModel.DataAnnotations;

namespace Sentinel.API.DTOs;

public record RegisterBrowserAgentRequest(
    [Required] string Name,
    string? IpAddress
);

public record BrowserLogRequest(
    [Required] Guid AgentId,
    [Required] string Level,
    [Required] string Message,
    [Required] string Source,
    string? StackTrace,
    Dictionary<string, object>? Properties
);

public record BrowserHeartbeatRequest(
    [Required] Guid AgentId,
    [Required] string Hostname
);
