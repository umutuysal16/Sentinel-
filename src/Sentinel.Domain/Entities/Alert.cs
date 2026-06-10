using Sentinel.Domain.Common;
using Sentinel.Domain.Enums;
using Sentinel.Domain.Events;

namespace Sentinel.Domain.Entities;

public class Alert : AggregateRoot
{
    public Guid LogEntryId { get; private set; }
    public int RiskScore { get; private set; }
    public AlertSeverity Severity { get; private set; }
    public AlertCategory Category { get; private set; }
    public string AiExplanation { get; private set; } = null!;
    public bool IsAcknowledged { get; private set; }
    public DateTime? AcknowledgedAt { get; private set; }
    public string? AcknowledgedBy { get; private set; }

    public LogEntry LogEntry { get; private set; } = null!;

    private Alert() { }

    public static Alert Create(
        Guid logEntryId,
        int riskScore,
        AlertCategory category,
        string aiExplanation)
    {
        if (riskScore < 1 || riskScore > 10)
            throw new ArgumentOutOfRangeException(nameof(riskScore), "Risk score must be between 1 and 10.");

        var alert = new Alert
        {
            LogEntryId = logEntryId,
            RiskScore = riskScore,
            Severity = CalculateSeverity(riskScore),
            Category = category,
            AiExplanation = aiExplanation,
            IsAcknowledged = false
        };

        alert.AddDomainEvent(new AlertCreatedEvent(
            alert.Id,
            alert.RiskScore,
            alert.Severity,
            alert.Category));

        return alert;
    }

    public void Acknowledge(string userName)
    {
        if (IsAcknowledged) return;

        IsAcknowledged = true;
        AcknowledgedAt = DateTime.UtcNow;
        AcknowledgedBy = userName;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new AlertAcknowledgedEvent(Id, userName));
    }

    private static AlertSeverity CalculateSeverity(int riskScore) => riskScore switch
    {
        <= 3 => AlertSeverity.Low,
        <= 5 => AlertSeverity.Medium,
        <= 8 => AlertSeverity.High,
        _ => AlertSeverity.Critical
    };
}
