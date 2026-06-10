using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sentinel.Application.Commands.IngestLog;
using Sentinel.Application.Queries.GetLogById;
using Sentinel.Application.Queries.GetLogs;

namespace Sentinel.API.Controllers;

[ApiController]
[Route("api/logs")]
public class LogEntriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public LogEntriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetLogs(
        [FromQuery] Guid? agentId,
        [FromQuery] int? level,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        var result = await _mediator.Send(new GetLogsQuery(agentId, level, page, pageSize));
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetLogById(Guid id)
    {
        var result = await _mediator.Send(new GetLogByIdQuery(id));
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> IngestLog([FromBody] IngestLogCommand command)
    {
        var logEntryId = await _mediator.Send(command);
        return Ok(new { logEntryId });
    }
}
