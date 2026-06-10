using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sentinel.API.DTOs;
using Sentinel.Application.Commands.IngestLog;
using Sentinel.Application.Commands.RegisterAgent;
using Sentinel.Application.Commands.UpdateAgentStatus;

namespace Sentinel.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BrowserLogsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<BrowserLogsController> _logger;

    public BrowserLogsController(IMediator mediator, ILogger<BrowserLogsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAgent([FromBody] RegisterBrowserAgentRequest request)
    {
        var agentId = await _mediator.Send(new RegisterAgentCommand(
            request.Name,
            "BrowserPlugin",
            request.IpAddress));

        return Ok(new { AgentId = agentId });
    }

    [HttpPost("heartbeat")]
    public async Task<IActionResult> Heartbeat([FromBody] BrowserHeartbeatRequest request)
    {
        await _mediator.Send(new UpdateAgentStatusCommand(
            request.AgentId,
            request.Hostname,
            "BrowserPlugin"));

        return Ok(new { Acknowledged = true });
    }

    [HttpPost]
    public async Task<IActionResult> IngestLog([FromBody] BrowserLogRequest request)
    {
        try
        {
            var parsedLevel = Enum.TryParse<Sentinel.Domain.Enums.LogLevel>(request.Level, true, out var levelEnum)
                ? (int)levelEnum
                : (int)Sentinel.Domain.Enums.LogLevel.Information;

            var logEntryId = await _mediator.Send(new IngestLogCommand(
                request.AgentId,
                parsedLevel,
                request.Source,
                request.Message,
                request.StackTrace,
                request.Properties,
                DateTime.UtcNow));

            return Ok(new { Success = true, LogEntryId = logEntryId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing browser log from Agent {AgentId}", request.AgentId);
            return StatusCode(500, new { Success = false, Error = "Internal Server Error" });
        }
    }
}
