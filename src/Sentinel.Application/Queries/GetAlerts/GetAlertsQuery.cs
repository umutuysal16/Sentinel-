using MediatR;
using Sentinel.Application.Common;
using Sentinel.Application.DTOs;

namespace Sentinel.Application.Queries.GetAlerts;

public record GetAlertsQuery(
    string? Severity,
    bool? IsAcknowledged,
    int Page = 1,
    int PageSize = 50) : IRequest<PaginatedList<AlertDto>>;
