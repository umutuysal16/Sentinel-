using MediatR;
using Microsoft.EntityFrameworkCore;
using Sentinel.Application.Common;
using Sentinel.Application.DTOs;
using Sentinel.Application.Interfaces;
using Sentinel.Application.Mappings;
using Sentinel.Domain.Enums;

namespace Sentinel.Application.Queries.GetAlerts;

public class GetAlertsQueryHandler : IRequestHandler<GetAlertsQuery, PaginatedList<AlertDto>>
{
    private readonly IApplicationDbContext _dbContext;

    public GetAlertsQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PaginatedList<AlertDto>> Handle(GetAlertsQuery request, CancellationToken cancellationToken)
    {
        var query = _dbContext.Alerts.AsQueryable();

        if (!string.IsNullOrEmpty(request.Severity) &&
            Enum.TryParse<AlertSeverity>(request.Severity, true, out var severity))
        {
            query = query.Where(a => a.Severity == severity);
        }

        if (request.IsAcknowledged.HasValue)
            query = query.Where(a => a.IsAcknowledged == request.IsAcknowledged.Value);

        query = query.OrderByDescending(a => a.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var dtos = items.Select(a => a.ToDto()).ToList();

        return new PaginatedList<AlertDto>(dtos, totalCount, request.Page, request.PageSize);
    }
}
