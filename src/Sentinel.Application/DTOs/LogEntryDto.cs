namespace Sentinel.Application.DTOs;

public record LogEntryDto(
    Guid Id,
    Guid AgentId,
    string AgentName,
    string Source,
    string Level,
    string Message,
    string? StackTrace,
    Dictionary<string, object> Properties,
    DateTime Timestamp);
