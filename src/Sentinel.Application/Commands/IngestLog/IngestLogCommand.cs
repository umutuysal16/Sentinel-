using MediatR;

namespace Sentinel.Application.Commands.IngestLog;

public record IngestLogCommand(
    Guid AgentId,
    int Level,
    string Source,
    string Message,
    string? StackTrace,
    Dictionary<string, object>? Properties,
    DateTime Timestamp) : IRequest<Guid>;
