using MediatR;
using Microsoft.EntityFrameworkCore;
using Sentinel.Application.DTOs;
using Sentinel.Application.Interfaces;
using Sentinel.Application.Mappings;

namespace Sentinel.Application.Queries.GetAgents;

public class GetAgentsQueryHandler : IRequestHandler<GetAgentsQuery, List<AgentDto>>
{
    private readonly IApplicationDbContext _dbContext;

    public GetAgentsQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<AgentDto>> Handle(GetAgentsQuery request, CancellationToken cancellationToken)
    {
        var agents = await _dbContext.Agents
            .OrderBy(a => a.Name)
            .ToListAsync(cancellationToken);

        var result = new List<AgentDto>();
        foreach (var agent in agents)
        {
            var logCount = await _dbContext.LogEntries
                .CountAsync(l => l.AgentId == agent.Id, cancellationToken);
            result.Add(agent.ToDto(logCount));
        }

        return result;
    }
}
