namespace Sentinel.Application.DTOs;

public record AlertDto(
    Guid Id,
    Guid LogEntryId,
    int RiskScore,
    string Severity,
    string Category,
    string AiExplanation,
    bool IsAcknowledged,
    DateTime? AcknowledgedAt,
    string? AcknowledgedBy,
    DateTime CreatedAt);
