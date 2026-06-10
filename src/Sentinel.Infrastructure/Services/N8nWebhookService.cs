using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sentinel.Application.DTOs;
using Sentinel.Application.Interfaces;
using Sentinel.Domain.Enums;

namespace Sentinel.Infrastructure.Services;

public class N8nWebhookService : IAiAnalysisService
{
    private readonly HttpClient _httpClient;
    private readonly string _webhookUrl;
    private readonly ILogger<N8nWebhookService> _logger;

    public N8nWebhookService(HttpClient httpClient, IConfiguration configuration, ILogger<N8nWebhookService> logger)
    {
        _httpClient = httpClient;
        _webhookUrl = configuration["N8nSettings:WebhookUrl"] ?? "http://localhost:5678/webhook/analyze-log";
        _logger = logger;

        var timeout = configuration.GetValue<int>("N8nSettings:TimeoutSeconds", 30);
        _httpClient.Timeout = TimeSpan.FromSeconds(timeout);
    }

    private static readonly SemaphoreSlim _rateLimitSemaphore = new SemaphoreSlim(1, 1);

    public async Task<AiAnalysisResult> AnalyzeLogAsync(
        Guid logEntryId, string message, string source, string level,
        CancellationToken cancellationToken = default)
    {
        await _rateLimitSemaphore.WaitAsync(cancellationToken);
        try
        {
            var payload = new
            {
                logEntryId = logEntryId.ToString(),
                message,
                source,
                level
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _logger.LogInformation("Sending log to n8n for AI analysis. LogEntryId: {LogEntryId}", logEntryId);

            var response = await _httpClient.PostAsync(_webhookUrl, content, cancellationToken);

            // Google Gemini API limit is ~15 RPM. Delay 4.5 seconds to guarantee we stay under the limit.
            await Task.Delay(4500, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("n8n webhook returned {StatusCode} for LogEntryId: {LogEntryId}",
                    response.StatusCode, logEntryId);
                return new AiAnalysisResult(false, 0, AlertCategory.AnomalousPattern, string.Empty);
            }

            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonSerializer.Deserialize<N8nResponse>(responseBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (result is null)
                return new AiAnalysisResult(false, 0, AlertCategory.AnomalousPattern, string.Empty);

            var riskScore = Math.Clamp(result.RiskScore, 1, 10);
            var category = Enum.TryParse<AlertCategory>(result.Category, true, out var cat)
                ? cat
                : AlertCategory.AnomalousPattern;

            return new AiAnalysisResult(true, riskScore, category, result.Explanation ?? "No explanation provided.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to call n8n webhook for LogEntryId: {LogEntryId}", logEntryId);
            return new AiAnalysisResult(false, 0, AlertCategory.AnomalousPattern, string.Empty);
        }
        finally
        {
            _rateLimitSemaphore.Release();
        }
    }

    private record N8nResponse(int RiskScore, string? Category, string? Explanation);
}
