using MediatR;
using Microsoft.EntityFrameworkCore;
using Sentinel.Application.Common.Exceptions;
using Sentinel.Application.DTOs;
using Sentinel.Application.Interfaces;
using Sentinel.Application.Mappings;

namespace Sentinel.Application.Queries.GetAlertById;

public class GetAlertByIdQueryHandler : IRequestHandler<GetAlertByIdQuery, AlertDetailDto>
{
    private readonly IApplicationDbContext _dbContext;

    public GetAlertByIdQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AlertDetailDto> Handle(GetAlertByIdQuery request, CancellationToken cancellationToken)
    {
        var alert = await _dbContext.Alerts
            .Include(a => a.LogEntry)
                .ThenInclude(l => l.Agent)
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Alert), request.Id);

        var relatedLog = alert.LogEntry.ToDto();

        var agent = alert.LogEntry.Agent;
        var agentLogCount = await _dbContext.LogEntries
            .CountAsync(l => l.AgentId == agent.Id, cancellationToken);
        var agentDto = agent.ToDto(agentLogCount);

        // Get last 10 logs from the same agent for timeline
        var timeline = await _dbContext.LogEntries
            .Include(l => l.Agent)
            .Where(l => l.AgentId == agent.Id)
            .OrderByDescending(l => l.Timestamp)
            .Take(10)
            .ToListAsync(cancellationToken);

        var timelineDtos = timeline.Select(l => l.ToDto()).ToList();

        return new AlertDetailDto(
            alert.Id,
            alert.RiskScore,
            alert.Severity.ToString(),
            alert.Category.ToString(),
            alert.AiExplanation,
            alert.IsAcknowledged,
            alert.AcknowledgedAt,
            alert.AcknowledgedBy,
            alert.CreatedAt,
            relatedLog,
            agentDto,
            timelineDtos);
    }
}
