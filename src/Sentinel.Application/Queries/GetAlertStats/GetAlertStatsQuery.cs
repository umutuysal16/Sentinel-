using MediatR;
using Sentinel.Application.DTOs;

namespace Sentinel.Application.Queries.GetAlertStats;

public record GetAlertStatsQuery() : IRequest<AlertStatsDto>;
