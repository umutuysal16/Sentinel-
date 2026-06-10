using MediatR;
using Microsoft.EntityFrameworkCore;
using Sentinel.Application.Common;
using Sentinel.Application.DTOs;
using Sentinel.Application.Interfaces;
using Sentinel.Application.Mappings;
using Sentinel.Domain.Enums;

namespace Sentinel.Application.Queries.GetLogs;

public class GetLogsQueryHandler : IRequestHandler<GetLogsQuery, PaginatedList<LogEntryDto>>
{
    private readonly IApplicationDbContext _dbContext;

    public GetLogsQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PaginatedList<LogEntryDto>> Handle(GetLogsQuery request, CancellationToken cancellationToken)
    {
        var query = _dbContext.LogEntries
            .Include(l => l.Agent)
            .AsQueryable();

        if (request.AgentId.HasValue)
            query = query.Where(l => l.AgentId == request.AgentId.Value);

        if (request.MinLevel.HasValue)
            query = query.Where(l => (int)l.Level >= request.MinLevel.Value);

        query = query.OrderByDescending(l => l.Timestamp);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var dtos = items.Select(l => l.ToDto()).ToList();

        return new PaginatedList<LogEntryDto>(dtos, totalCount, request.Page, request.PageSize);
    }
}
