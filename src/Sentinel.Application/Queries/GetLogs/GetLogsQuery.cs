using MediatR;
using Sentinel.Application.Common;
using Sentinel.Application.DTOs;

namespace Sentinel.Application.Queries.GetLogs;

public record GetLogsQuery(
    Guid? AgentId,
    int? MinLevel,
    int Page = 1,
    int PageSize = 50) : IRequest<PaginatedList<LogEntryDto>>;
