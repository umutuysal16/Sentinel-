using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sentinel.Application.Queries.GetDashboardSummary;

namespace Sentinel.API.Controllers;

[ApiController]
[Route("api/dashboard")]
public class DashboardController : ControllerBase
{
    private readonly IMediator _mediator;

    public DashboardController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        var result = await _mediator.Send(new GetDashboardSummaryQuery());
        return Ok(result);
    }
}
