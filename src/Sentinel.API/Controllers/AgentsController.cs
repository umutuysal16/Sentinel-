using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sentinel.Application.Commands.RegisterAgent;
using Sentinel.Application.Queries.GetAgents;

namespace Sentinel.API.Controllers;

[ApiController]
[Route("api/agents")]
public class AgentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AgentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAgents()
    {
        var result = await _mediator.Send(new GetAgentsQuery());
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> RegisterAgent([FromBody] RegisterAgentCommand command)
    {
        var agentId = await _mediator.Send(command);
        return Ok(new { agentId });
    }
}
