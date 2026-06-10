using MediatR;
using Sentinel.Application.DTOs;

namespace Sentinel.Application.Queries.GetAlertById;

public record GetAlertByIdQuery(Guid Id) : IRequest<AlertDetailDto>;
