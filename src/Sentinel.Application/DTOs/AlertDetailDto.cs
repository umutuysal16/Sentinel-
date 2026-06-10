namespace Sentinel.Application.DTOs;

public record AlertDetailDto(
    Guid Id,
    int RiskScore,
    string Severity,
    string Category,
    string AiExplanation,
    bool IsAcknowledged,
    DateTime? AcknowledgedAt,
    string? AcknowledgedBy,
    DateTime CreatedAt,
    LogEntryDto RelatedLog,
    AgentDto Agent,
    List<LogEntryDto> AgentTimeline);
