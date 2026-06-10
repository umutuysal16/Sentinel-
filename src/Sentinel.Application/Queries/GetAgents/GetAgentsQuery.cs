using MediatR;
using Sentinel.Application.DTOs;

namespace Sentinel.Application.Queries.GetAgents;

public record GetAgentsQuery() : IRequest<List<AgentDto>>;
