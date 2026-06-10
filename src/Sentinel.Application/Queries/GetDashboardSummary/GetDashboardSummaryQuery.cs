using MediatR;
using Sentinel.Application.DTOs;

namespace Sentinel.Application.Queries.GetDashboardSummary;

public record GetDashboardSummaryQuery() : IRequest<DashboardSummaryDto>;
