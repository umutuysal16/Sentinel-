using Sentinel.Domain.Enums;

namespace Sentinel.Application.DTOs;

public record AiAnalysisResult(
    bool Success,
    int RiskScore,
    AlertCategory Category,
    string Explanation);
