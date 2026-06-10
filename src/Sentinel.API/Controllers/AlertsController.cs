using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sentinel.Application.Commands.AcknowledgeAlert;
using Sentinel.Application.Commands.ClearAlerts;
using Sentinel.Application.Queries.GetAlertById;
using Sentinel.Application.Queries.GetAlerts;
using Sentinel.Application.Queries.GetAlertStats;

namespace Sentinel.API.Controllers;

[ApiController]
[Route("api/alerts")]
public class AlertsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AlertsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAlerts(
        [FromQuery] string? severity,
        [FromQuery] bool? acknowledged,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        var result = await _mediator.Send(new GetAlertsQuery(severity, acknowledged, page, pageSize));
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetAlertById(Guid id)
    {
        var result = await _mediator.Send(new GetAlertByIdQuery(id));
        return Ok(result);
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetAlertStats()
    {
        var result = await _mediator.Send(new GetAlertStatsQuery());
        return Ok(result);
    }

    [HttpPost("{id:guid}/acknowledge")]
    public async Task<IActionResult> AcknowledgeAlert(Guid id, [FromQuery] string userName = "admin")
    {
        await _mediator.Send(new AcknowledgeAlertCommand(id, userName));
        return Ok(new { message = "Alert acknowledged." });
    }

    [HttpDelete]
    public async Task<IActionResult> ClearAlerts([FromQuery] bool onlyAcknowledged = true)
    {
        var deletedCount = await _mediator.Send(new ClearAlertsCommand(onlyAcknowledged));
        return Ok(new { deletedCount, message = $"{deletedCount} alerts deleted." });
    }
}
