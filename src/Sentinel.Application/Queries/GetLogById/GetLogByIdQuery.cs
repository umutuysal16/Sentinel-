using MediatR;
using Sentinel.Application.DTOs;

namespace Sentinel.Application.Queries.GetLogById;

public record GetLogByIdQuery(Guid Id) : IRequest<LogEntryDto>;
