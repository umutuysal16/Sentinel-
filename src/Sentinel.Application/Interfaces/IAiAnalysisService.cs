using Sentinel.Application.DTOs;

namespace Sentinel.Application.Interfaces;

public interface IAiAnalysisService
{
    Task<AiAnalysisResult> AnalyzeLogAsync(Guid logEntryId, string message, string source, string level, CancellationToken cancellationToken = default);
}
