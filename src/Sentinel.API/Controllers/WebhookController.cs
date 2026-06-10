using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sentinel.Application.Commands.CreateAlert;
using Sentinel.Domain.Enums;

namespace Sentinel.API.Controllers;

[ApiController]
[Route("api/webhook")]
public class WebhookController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<WebhookController> _logger;

    public WebhookController(IMediator mediator, ILogger<WebhookController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost("n8n-callback")]
    public async Task<IActionResult> N8nCallback([FromBody] N8nCallbackRequest request)
    {
        _logger.LogInformation("n8n callback received. LogEntryId: {LogEntryId}, RiskScore: {RiskScore}",
            request.LogEntryId, request.RiskScore);

        var category = Enum.TryParse<AlertCategory>(request.Category, true, out var cat)
            ? cat
            : AlertCategory.AnomalousPattern;

        var alertId = await _mediator.Send(new CreateAlertCommand(
            request.LogEntryId,
            Math.Clamp(request.RiskScore, 1, 10),
            category,
            request.Explanation ?? "No explanation provided."));

        return Ok(new { alertId });
    }
}

public record N8nCallbackRequest(
    Guid LogEntryId,
    int RiskScore,
    string? Category,
    string? Explanation);
